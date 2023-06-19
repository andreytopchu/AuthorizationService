using System.Linq;
using System.Threading.Tasks;
using Identity.Dal;
using IdentityDbSeeder.SeedData;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityDbSeeder.Seeder;

public class ConfigurationDbSeeder : BaseEfSeeder<IdentityConfigurationDbContext>, IDbSeeder
{
    public ConfigurationDbSeeder(IdentityConfigurationDbContext dbContext) : base(dbContext)
    {
    }

    protected override async Task EnsureSeedData()
    {
        Log.Debug("Clients being populated");
        foreach (var client in SeedConfig.Clients.ToList())
        {
            var exClient = DbContext.Clients.FirstOrDefault(x => x.ClientId == client.ClientId);
            if (exClient == null)
            {
                DbContext.Clients.Add(client.ToEntity());
            }
            else
            {
                DbContext.Clients.Remove(exClient);
                DbContext.Clients.Add(client.ToEntity());
            }
        }

        Log.Debug("IdentityResources being populated");
        foreach (var resource in SeedConfig.IdentityResources.ToList().Where(resource => !DbContext.IdentityResources.Any(x => x.Name == resource.Name)))
        {
            await DbContext.IdentityResources.AddAsync(resource.ToEntity());
        }

        Log.Debug("ApiScopes being populated");
        foreach (var resource in SeedConfig.ApiScopes.ToList().Where(resource => !DbContext.ApiScopes.Any(x => x.Name == resource.Name)))
        {
            await DbContext.ApiScopes.AddAsync(resource.ToEntity());
        }

        Log.Debug("ApiResources being populated");
        foreach (var resource in SeedConfig.ApiResources.ToList())
        {
            var apiResource = DbContext.ApiResources
                .Include(x => x.Scopes)
                .FirstOrDefault(x => x.Name == resource.Name);

            if (apiResource == null)
            {
                await DbContext.ApiResources.AddAsync(resource.ToEntity());
            }
            else
            {
                if (apiResource.Scopes.Select(x => x.Scope).Intersect(resource.Scopes).Count() ==
                    resource.Scopes.Count)
                {
                    continue;
                }


                apiResource.Scopes.Clear();
                apiResource.Scopes.AddRange(resource.Scopes.Select(x => new ApiResourceScope {Scope = x}));
            }
        }

        await DbContext.SaveChangesAsync();
    }
}