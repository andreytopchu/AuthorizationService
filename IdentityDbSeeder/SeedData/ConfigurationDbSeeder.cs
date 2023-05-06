using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Shared.BaseDbSeeder.Seeder;

namespace IdentityDbSeeder.SeedData
{
    public class ConfigurationDbSeeder : BaseEfSeeder<ConfigurationDbContext>, IDbSeeder
    {
        public ConfigurationDbSeeder(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        protected override Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }
    }
}