﻿using System;
using System.Threading.Tasks;
using IdentityDbSeeder.Seeder;

namespace IdentityDbSeeder;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await SeedRunner.Seed<ConfigurationDbSeeder>(args);

        // аргументы не передаем, иначе затрем БД(из предыдущего сида), если указан ключ --drop
        await SeedRunner.Seed<PersistedGrantDbSeeder>(Array.Empty<string>());
        await SeedRunner.Seed<Seeder.IdentityDbSeeder>(Array.Empty<string>());
        await SeedRunner.Seed<SecurityTokenDbSeeder>(Array.Empty<string>());
    }
}