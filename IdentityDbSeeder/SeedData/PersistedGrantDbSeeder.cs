using System.Threading.Tasks;
using IdentityDbSeeder.Seeder;
using IdentityServer4.EntityFramework.DbContexts;

namespace IdentityDbSeeder.SeedData
{
    public class PersistedGrantDbSeeder : BaseEfSeeder<PersistedGrantDbContext>, IDbSeeder
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