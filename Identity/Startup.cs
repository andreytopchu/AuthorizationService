using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Dex.Extensions;
using Dex.MassTransit.Rabbit;
using Dex.SecurityToken.RedisStorage;
using Dex.SecurityTokenProvider.Extensions;
using Dex.SecurityTokenProvider.Options;
using Identity.Application.Abstractions.Models.Command.Email;
using Identity.Application.Abstractions.Options;
using Identity.Application.Extensions;
using Identity.Consumers;
using Identity.Dal;
using Identity.Dal.Extensions;
using Identity.ExceptionFilter;
using Identity.Logger;
using Identity.Options;
using Identity.Services;
using Identity.Services.Extensions;
using Identity.Swagger;
using IdentityModel.AspNetCore.AccessTokenValidation;
using IdentityModel.AspNetCore.OAuth2Introspection;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Identity;

public class Startup
{
    private const string RootPath = "/identity";
    private const string ApplicationName = "s.identity";
    private IWebHostEnvironment Environment { get; }
    private IConfiguration Configuration { get; }

    /// <summary>
    /// Будет проинициализирован только после вызова Configure
    /// </summary>
    private IServiceProvider? Sp { get; set; }

    private AuthorizationSettings AuthorizationSettings =>
        Sp?.GetRequiredService<IOptions<AuthorizationSettings>>().Value
        ?? throw new InvalidOperationException("Can't get AuthorizationSettings, please InitServiceProvider() from Configure()");

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public void Configure(IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        InitServiceProvider(app.ApplicationServices);

        app.UsePathBase(new PathString(RootPath)); // for proxy, remove prefix from request (/identity/get == /get)

        if (Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }

        // process exceptions
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // request logger
        app.UseRequestLogger();

        app.UseStaticFiles();

        app.UseIdentityServer();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        // check automapper config
        var provider = app.ApplicationServices.GetRequiredService<IConfigurationProvider>();
        provider.AssertConfigurationIsValid();

        app.UseEndpoints(builder => { builder.MapControllers(); });

        //swagger
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // configuration
        services.AddOptions<IdentityOptions>().Configure(options => options.ProviderName = "identity-providerName");
        services.Configure<TokenOptions>(Configuration.GetSection(nameof(TokenOptions)));
        services.Configure<EmailOptions>(Configuration.GetSection(nameof(EmailOptions)));
        services.Configure<RedisConfigurationOptions>(Configuration.GetSection(nameof(RedisConfigurationOptions)));
        services.Configure<RabbitMqOptions>(Configuration.GetSection(nameof(RabbitMqOptions)));

        // controllers
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        // identity
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = false;
                options.Events.RaiseSuccessEvents = false;

                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = false;
            })
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore<IdentityConfigurationDbContext>(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString, optionsBuilder =>
                    {
                        optionsBuilder.MigrationsAssembly(typeof(IdentityConfigurationDbContext).Assembly.FullName);
                        optionsBuilder.EnableRetryOnFailure();
                    });
            })
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(connectionString,
                        optionsBuilder =>
                        {
                            optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
                            optionsBuilder.EnableRetryOnFailure();
                        });

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 3600;
            })
            .AddProfileService<ProfileService>()
            .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
            .AddConfigurationStoreCache();

        // Signing Key
        var signTokenKeyFileName = Environment.IsProduction() ? "prod-sign-token-key.json" : "sign-token-key.json";
        var rsa = RSA.Create(JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(signTokenKeyFileName)));
        builder.AddSigningCredential(new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256));

        services.AddAuthentication();

        //services
        services.AddIdentityServices(Environment);

        // core
        services.AddIdentityDal(Configuration.GetConnectionString("DefaultConnection")!);
        services.AddIdentityUseCases();

        // swagger
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(ConfigureSwagger);

        //token provider
        services.RegisterDbContext<SecurityTokenDbContext>(Configuration.GetConnectionString("SecurityProviderConnection"));
        services.AddSecurityTokenProvider<TokenRedisStorageProvider>(Configuration.GetSection(nameof(TokenProviderOptions)));
        services.AddTokenRedisStorageServices();
        services.AddDataProtection().PersistKeysToDbContext<SecurityTokenDbContext>().SetApplicationName(ApplicationName);

        // authentication
        services.Configure<AuthorizationSettings>(Configuration.GetSection(nameof(AuthorizationSettings)));
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, ConfigureJwt)
            .AddOAuth2Introspection("introspection", ConfigureIntrospection);

        // authorization
        services.AddAuthorization(ConfigureAuthorization);

        // StackExchange redis cache
        ConfigureRedisStackExchange(services);

        // masstransit
        services.AddMassTransit(x =>
        {
            ConfigureConsumer<EmailNotificationConsumer>(x);

            // register send endpoints
            x.RegisterBus((context, configurator) =>
            {
                context.RegisterSendEndPoint<ISendEmailCommand>();

                context.RegisterReceiveEndpoint<EmailNotificationConsumer, ISendEmailCommand>(configurator);
            });
        });
    }

    private void InitServiceProvider(IServiceProvider serviceProvider)
    {
        Sp = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    private void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.EnableAnnotations();
        // Игнорим методы не помеченные явно HTTP методом (GET, POST и тд).
        options.DocInclusionPredicate((_, desc) => desc.HttpMethod != null);

        options.TagActionsBy(
            api =>
            {
                if (api.GroupName != null && !Regex.IsMatch(api.GroupName, @"^\d"))
                {
                    return new[] {api.GroupName};
                }

                if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    return new[] {controllerActionDescriptor.ControllerName};
                }

                throw new InvalidOperationException("Unable to determine tag for endpoint.");
            });
    }

    private void ConfigureJwt(JwtBearerOptions options)
    {
        // options.MetadataAddress = "https://<base.url>/identity/.well-known/openid-configuration";
        options.Authority = AuthorizationSettings.AuthorityUrl.ToString();
        options.Audience = AuthorizationSettings.ApiResource;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = 1.Minutes()
        };

        // если токен не содержит точку (это референс токен), уходим на introspection endpoint
        options.ForwardDefaultSelector = delegate(HttpContext context)
        {
            var nextSchema = AuthorizationSettings.UseReferenceToken
                ? Selector.ForwardReferenceToken("introspection")(context)
                : null;
            return nextSchema;
        };
    }

    private void ConfigureIntrospection(OAuth2IntrospectionOptions options)
    {
        options.Authority = AuthorizationSettings.AuthorityUrl.ToString();
        options.EnableCaching = AuthorizationSettings.IntrospectionCacheTimeSeconds > 0;
        options.CacheDuration = TimeSpan.FromSeconds(Math.Max(AuthorizationSettings.IntrospectionCacheTimeSeconds, 10));
        options.ClientId = AuthorizationSettings.ApiResource;
        options.ClientSecret = AuthorizationSettings.ApiResourceSecret;
        options.SaveToken = true;
    }

    protected virtual void ConfigureAuthorization(AuthorizationOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (!AuthorizationSettings.ApiScopeRequired.IsNullOrEmpty())
        {
            options.AddPolicy("ApiScopeRequired", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", AuthorizationSettings.ApiScopeRequired!);
                Log.Information("Scope policy added - {PolicyName}", "ApiScopeRequired");
            });
        }

        if (!AuthorizationSettings.ApiScopeOnlyRequired.IsNullOrEmpty())
        {
            foreach (var scope in AuthorizationSettings.ApiScopeOnlyRequired!)
            {
                var policyName = "only-" + scope;
                options.AddPolicy(policyName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", scope);
                });
                Log.Information("Scope policy added - {PolicyName}", policyName);
            }
        }

        if (!AuthorizationSettings.ApiPolicies.IsNullOrEmpty())
        {
            const string claimType = "policy";

            foreach (var apiPolicy in AuthorizationSettings.ApiPolicies)
            {
                // fullAccess должен игнорироваться
                if (apiPolicy == "fullAccess")
                    continue;

                options.AddPolicy(apiPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(claimType);

                    var fullPolicyName = string.Join('_', AuthorizationSettings.ApiResource, apiPolicy);
                    //write включает read
                    if (fullPolicyName.EndsWith(".read", StringComparison.InvariantCulture))
                    {
                        var basePolicy = fullPolicyName.Replace(".read", string.Empty, StringComparison.InvariantCulture);
                        policy.RequireAssertion(context =>
                        {
                            var policyValue = context.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
                            return policyValue != null &&
                                   (policyValue.Contains(fullPolicyName, StringComparison.InvariantCulture) ||
                                    policyValue.Contains($"{basePolicy}.write",
                                        StringComparison.InvariantCulture) ||
                                    policyValue.Contains("fullAccess", StringComparison.InvariantCulture));
                        });
                    }
                    else
                    {
                        policy.RequireAssertion(context =>
                        {
                            var policyValue = context.User.Claims.FirstOrDefault(x => x.Type == claimType)?.Value;
                            return policyValue != null &&
                                   (policyValue.Contains(fullPolicyName, StringComparison.InvariantCulture) ||
                                    policyValue.Contains("fullAccess", StringComparison.InvariantCulture));
                        });
                    }
                });

                Log.Information("Policy added - {PolicyName}", apiPolicy);
            }
        }
    }

    private void ConfigureRedisStackExchange(IServiceCollection services)
    {
        // StackExchange redis cache
        var redisOptions = new RedisConfigurationOptions();
        Configuration.GetSection(nameof(RedisConfigurationOptions)).Bind(redisOptions);
        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions();
            redisOptions.EndPoints.ForEach(x => options.ConfigurationOptions.EndPoints.Add(x));
            if (redisOptions.Password != null) options.ConfigurationOptions.Password = redisOptions.Password;
            if (redisOptions.CommandMap == nameof(CommandMap.Sentinel))
            {
                options.ConfigurationOptions.CommandMap = CommandMap.Sentinel;
            }
        });
    }

    private static void ConfigureConsumer<TConsumer>(IBusRegistrationConfigurator serviceCollectionBusConfigurator)
        where TConsumer : class, IConsumer
    {
        serviceCollectionBusConfigurator.AddConsumer<TConsumer>(configurator =>
        {
            configurator.UseConcurrencyLimit(1);
            configurator.UseMessageRetry(retryConfigurator => retryConfigurator.Interval(10, 10.Seconds()));
        });
    }
}