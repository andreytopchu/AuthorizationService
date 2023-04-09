using System;
using System.Threading.Tasks;
using IdentityDbSeeder.SeedData;
using Shared.BaseDbSeeder;

namespace IdentityDbSeeder
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await SeedRunner.Seed<Startup, ConfigurationDbSeeder>(args);
            // аргументы не передаем, иначе затрем БД(из предыдущего сида), если указан ключ --drop
            await SeedRunner.Seed<Startup, PersistedGrantDbSeeder>(Array.Empty<string>());
        }
    }
}