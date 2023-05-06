using System.Threading.Tasks;

namespace Shared.BaseDbSeeder.Seeder
{
    public interface IDbSeeder
    {
        Task RunAsync(bool ensureDeleted);
    }
}