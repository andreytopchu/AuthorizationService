using System.Threading.Tasks;
using Identity.Dal;

namespace IdentityDbSeeder.Seeder
{
    public class SecurityTokenDbSeeder : BaseEfSeeder<SecurityTokenDbContext>, IDbSeeder
    {
        public SecurityTokenDbSeeder(SecurityTokenDbContext dbContext) : base(dbContext)
        {
        }

        protected override Task EnsureSeedData()
        {
            return Task.CompletedTask;
        }
    }
}