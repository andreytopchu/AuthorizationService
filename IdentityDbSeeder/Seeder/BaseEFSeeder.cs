using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace IdentityDbSeeder.Seeder;

public abstract class BaseEfSeeder<TDbContext> : BaseSeeder<TDbContext>
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }

    protected BaseEfSeeder(TDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    protected override bool IsTransientException(Exception exception)
    {
        return exception is NpgsqlException {IsTransient: true};
    }

    protected override Task EnsureCreated()
    {
        return DbContext.Database.EnsureCreatedAsync();
    }

    protected override Task EnsureDeleted()
    {
        return DbContext.Database.EnsureDeletedAsync();
    }

    protected override async Task MigrateAsync()
    {
        await DbContext.Database.MigrateAsync();

        // Если миграция зарегистрировала новые типы в БД то они МОГУТ быть не видны в ближайших запросах.
        // Поэтому говорим БД сделать Flush.
        if (DbContext.Database.GetDbConnection() is NpgsqlConnection npg)
        {
            await npg.OpenAsync(); // Диспозить не должны.
            npg.ReloadTypes();
        }
    }
}