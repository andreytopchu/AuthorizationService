using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Identity.HealthCheck;
using Identity.Mapping;
using Identity.Options;
using Identity.Services;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shared;
using Shared.Extensions;
using Shared.HealthCheck;

namespace Identity
{
    public class Startup : BaseStartup
    {
        private const string RootPath = "/identity";

        public Startup(IWebHostEnvironment environment, IConfiguration configuration) : base(environment, configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // configuration
            services.AddOptionsWithDataAnnotationsValidation<GrpcClientOptions>(Configuration.GetSection(nameof(GrpcClientOptions)));

            services.AddOptionsWithDataAnnotationsValidation<IdentityOptions>()
                .Configure(options => options.ProviderName = "template-providerName");


            // checks
            services.AddHealthChecks()
                .AddDbContextCheck<PersistedGrantDbContext>()
                .AddDbContextCheck<ConfigurationDbContext>()
                .AddCheck<GrpcHealthCheck<UserStoreHealthClient>>("user store");

            // controllers
            if (Environment.IsDevelopment()) // пока не предусмотрена интерактивная часть, отключаем вьюхи в проде
            {
                services.AddControllersWithViews();
            }
            else
            {
                services.AddControllers();
            }

            // grpc services
            ConfigureGrpcServices(services);

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
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseNpgsql(connectionString, optionsBuilder =>
                        {
                            optionsBuilder.MigrationsAssembly(GetType().Assembly.FullName);
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

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // register your IdentityServer with Google at https://console.developers.google.com
                    // enable the Google+ API
                    // set the redirect URI to https://localhost:15001/signin-google
                    options.ClientId = "copy client ID from Google here";
                    options.ClientSecret = "copy client secret from Google here";
                });

            // automapper
            services.AddAutoMapper(expression =>
            {
                expression.AddMaps(Assembly.GetEntryAssembly());
                expression.AddProfile(new MainProfile());
            });

            // services
            base.ConfigureServices(services);
        }

        private static void ConfigureGrpcServices(IServiceCollection services)
        {
            services.AddGrpcClient<UserStore.UserStoreClient>("web.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.ProfileServiceUri; // такойже как и мобайл, аутентификация там же
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "web.client.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("mobile.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.ProfileServiceUri;
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "mobile.client.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("mobile.client.long.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.ProfileServiceUri;
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "mobile.client.long.userstore" };
            });
            services.AddGrpcClient<UserStore.UserStoreClient>("admin.client.userstore", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.AdminServiceUri;
                o.Creator = invoker => new UserStore.UserStoreClient(invoker) { Name = "admin.client.userstore" };
            });

            // grpc health checks
            services.AddGrpcClient<UserStoreHealthClient>("profile.client.health", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.ProfileServiceUri;
            });
            services.AddGrpcClient<UserStoreHealthClient>("admin.client.health", (p, o) =>
            {
                var options = p.GetRequiredService<IOptions<GrpcClientOptions>>();
                o.Address = options.Value.AdminServiceUri;
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitServiceProvider(app.ApplicationServices);

            app.UsePathBase(new PathString(RootPath)); // for proxy, remove prefix from request (/identity/get == /get)

            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }

            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseRouting();
            app.UseAuthentication();

            app.UseEndpoints(builder =>
            {
                ConfigureSystemEndpoints(builder);
                builder.MapDefaultControllerRoute();
            });
        }
    }
}