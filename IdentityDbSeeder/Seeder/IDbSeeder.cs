using System.Threading.Tasks;

namespace IdentityDbSeeder.Seeder
{
    public interface IDbSeeder
    {
        Task RunAsync(bool ensureDeleted);
    }
}