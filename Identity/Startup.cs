﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Identity.Application.Extensions;
using Identity.Dal.Extensions;
using Identity.ExceptionFilter;
using Identity.Options;
using Identity.Services;
using Identity.Services.Extensions;
using Identity.Swagger;
using Microsoft.AspNetCore.Builder;
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
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Identity;

public class Startup
{
    private const string RootPath = "/identity";
    private IWebHostEnvironment Environment { get; }
    private IConfiguration Configuration { get; }

    public Startup(IWebHostEnvironment environment, IConfiguration configuration)
    {
        Environment = environment;
        Configuration = configuration;
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UsePathBase(new PathString(RootPath)); // for proxy, remove prefix from request (/identity/get == /get)

        // process exceptions
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseStaticFiles();

        app.UseIdentityServer();
        app.UseRouting();
        app.UseAuthentication();

        app.UseEndpoints(builder => { builder.MapDefaultControllerRoute(); });

        //swagger
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // configuration
        services.AddOptions<IdentityOptions>()
            .Configure(options => options.ProviderName = "template-providerName");

        // controllers
        services.AddControllersWithViews();

        services.AddIdentityServices();

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

        services.AddAuthentication();

        //services
        services.AddIdentityServices();

        // core
        services.AddIdentityDal(Configuration.GetConnectionString("DefaultConnection")!);
        services.AddIdentityUseCases();

        // swagger
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(ConfigureSwagger);
    }

    protected virtual void ConfigureSwagger(SwaggerGenOptions options)
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
}