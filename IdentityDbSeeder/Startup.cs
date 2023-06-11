﻿using System;
using Dex.Extensions;
using Identity.Dal;
using Identity.Dal.ConfigurationDb;
using Identity.Dal.Extensions;
using Identity.Dal.PersistedGrantDb;
using IdentityDbSeeder.SeedData;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityDbSeeder;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(HostBuilderContext builderContext)
    {
        Configuration = builderContext.Configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        AddDbContexts(services);

        services.AddScoped<ConfigurationDbSeeder>();
        services.AddScoped<PersistedGrantDbSeeder>();
        services.AddScoped<SeedData.IdentityDbSeeder>();
    }

    private void AddDbContexts(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        if (connectionString!.IsNullOrEmpty())
            throw new Exception("DefaultConnection is required");

        services.AddConfigurationDbContext(options =>
        {
            options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(typeof(InitConfigurationDbContext).Assembly.GetName().Name));
        });

        services.AddOperationalDbContext(options =>
        {
            options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString,
                optionsBuilder =>
                    optionsBuilder.MigrationsAssembly(typeof(InitPersistedGrantDbContext).Assembly.GetName().Name));
        });

        services.RegisterDbContext<IdentityDbContext>(connectionString!);
    }
}