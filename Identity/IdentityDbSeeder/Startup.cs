using Common.Logging;
using Dex.Extensions;
using Identity.Mapping;
using Identity.Migrations.ConfigurationDb;
using Identity.Migrations.PersistedGrantDb;
using IdentityDbSeeder.SeedData;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.BaseDbSeeder;

namespace IdentityDbSeeder
{
    public class Startup : BaseStartup
    {
        public Startup(HostBuilderContext builderContext) : base(builderContext)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(expression => expression.AddProfile(typeof(MainProfile)));

            AddDbContexts(services);

            services.AddScoped<ConfigurationDbSeeder>();
            services.AddScoped<PersistedGrantDbSeeder>();
        }

        private void AddDbContexts(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString.IsNullOrEmpty()) throw new ConfigurationException("DefaultConnection is required");

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
        }
    }
}