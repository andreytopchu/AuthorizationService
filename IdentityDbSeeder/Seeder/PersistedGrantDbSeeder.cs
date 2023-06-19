using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;

namespace IdentityDbSeeder.Seeder;

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