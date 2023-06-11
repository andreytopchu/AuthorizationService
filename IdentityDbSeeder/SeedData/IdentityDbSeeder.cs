using System.Threading.Tasks;
using Identity.Dal;
using IdentityDbSeeder.Seeder;

namespace IdentityDbSeeder.SeedData;

public class IdentityDbSeeder : BaseEfSeeder<IdentityDbContext>, IDbSeeder
{
    public IdentityDbSeeder(IdentityDbContext dbContext) : base(dbContext)
    {
    }

    protected override Task EnsureSeedData()
    {
        return Task.CompletedTask;
    }
}