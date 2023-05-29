using System;
using System.Threading.Tasks;
using Serilog;

namespace IdentityDbSeeder.Seeder;

/// <summary>
/// Сеятель данных, начальное наполнение.
/// </summary>
/// <typeparam name="TDbContext">Контекст данных.</typeparam>
public abstract class BaseSeeder<TDbContext>
{
    private readonly TimeSpan _delay;

    protected BaseSeeder(int delaySeconds = 2)
    {
        if (delaySeconds <= 0) throw new ArgumentOutOfRangeException(nameof(delaySeconds));

        _delay = TimeSpan.FromSeconds(Math.Max(1, delaySeconds));
    }

    public async Task RunAsync(bool ensureDeleted = false)
    {
        Log.Debug("Seeding database [{Database}]", typeof(TDbContext).FullName);

        var repeatCount = 10;
        Exception? lastException = null;

        do
        {
            Log.Debug("Try migrate database [{Database}]. Repeat: {RepeatCount}", typeof(TDbContext).FullName, repeatCount);

            try
            {
                if (ensureDeleted)
                {
                    await EnsureDeleted();
                }

                await MigrateAsync();
                await EnsureSeedData();

                Log.Debug("Migrate database [{Database}] completed", typeof(TDbContext).FullName);
                break;
            }
            catch (Exception ex) when (IsTransientException(ex))
            {
                lastException = ex;
                await Task.Delay(_delay);
            }
        } while (--repeatCount > 0);

        if (lastException != null)
        {
            throw new InvalidOperationException($"Migrate database [{typeof(TDbContext).FullName}] failed.",
                lastException);
        }
    }

    protected abstract bool IsTransientException(Exception exception);
    protected abstract Task EnsureCreated();

    protected abstract Task EnsureDeleted();

    protected abstract Task MigrateAsync();

    protected abstract Task EnsureSeedData();
}