using System.Linq;
using System.Threading.Tasks;
using Identity.Dal;
using IdentityDbSeeder.Seeder;
using IdentityServer4.EntityFramework.Mappers;
using Serilog;

namespace IdentityDbSeeder.SeedData;

public class ConfigurationDbSeeder : BaseEfSeeder<IdentityConfigurationDbContext>, IDbSeeder
{
    public ConfigurationDbSeeder(IdentityConfigurationDbContext dbContext) : base(dbContext)
    {
    }

    protected override async Task EnsureSeedData()
    {
        Log.Debug("IdentityResources being populated");
        foreach (var resource in SeedConfig.IdentityResources.ToList().Where(resource => !DbContext.IdentityResources.Any(x => x.Name == resource.Name)))
        {
            await DbContext.IdentityResources.AddAsync(resource.ToEntity());
        }
    }
}