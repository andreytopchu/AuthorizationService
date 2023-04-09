using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Shared.BaseDbSeeder.Seeder;

namespace IdentityDbSeeder.SeedData
{
    public class PersistedGrantDbSeeder : BaseEFSeeder<PersistedGrantDbContext>, IDbSeeder
    {
        public PersistedGrantDbSeeder(PersistedGrantDbContext context) : base(context)
        {
        }

        protected override Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }
    }
}