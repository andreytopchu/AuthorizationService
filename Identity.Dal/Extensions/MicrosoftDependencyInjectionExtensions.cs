using System;
using Identity.Abstractions;
using Identity.Abstractions.Repository;
using Identity.Dal.Interceptors;
using Identity.Dal.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;

namespace Identity.Dal.Extensions;

public static class MicrosoftDependencyInjectionExtensions
{
    private const int DefaultRetryDelay = 500;

    public static void RegisterDal<TDbContext, TReadDbContext>(this IServiceCollection services, string writeConnectionString,
        string? readConnectionString = null)
        where TDbContext : DbContext, IWriteDbContext, IUnitOfWork
        where TReadDbContext : DbContext, IReadDbContext
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (writeConnectionString == null) throw new ArgumentNullException(nameof(writeConnectionString));

        // dbContexts
        services.RegisterDbContext<TDbContext>(writeConnectionString);
        services.AddScoped<IWriteDbContext>(p => p.GetRequiredService<TDbContext>());
        services.AddScoped<IUnitOfWork>(p => p.GetRequiredService<TDbContext>());

        readConnectionString ??= writeConnectionString;
        services.RegisterReadDbContext<TReadDbContext>(readConnectionString);
        services.AddScoped<IReadDbContext>(p => p.GetRequiredService<TReadDbContext>());

        // default repositories
        services.RegisterGenericRepository();
    }

    public static void RegisterDbContext<TDbContext>(this IServiceCollection services, string connectionString,
        int maxRetryCount = 3, TimeSpan maxRetryDelay = default)
        where TDbContext : DbContext
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        services.RegisterRequiredServices();
        services.RegisterInterceptors();

        // only scoped, because interceptor may have scoped lifecycle
        services.AddDbContext<TDbContext>((sp, builder) =>
        {
            builder.UseNpgsql(connectionString, optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(typeof(TDbContext).Assembly.GetName().Name);
                optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                    maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(DefaultRetryDelay), null);
            });

            builder.AddInterceptors(sp.GetServices<IInterceptor>());
        });
    }

    public static void RegisterReadDbContext<TReadDbContext>(this IServiceCollection services, string connectionString,
        int maxRetryCount = 3, TimeSpan maxRetryDelay = default)
        where TReadDbContext : DbContext
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));

        services.RegisterRequiredServices();

        services.AddDbContextPool<TReadDbContext>(builder =>
        {
            builder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            builder.UseNpgsql(connectionString,
                optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                        maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(DefaultRetryDelay), null);
                });
        });
    }

    // ReSharper disable MemberCanBePrivate.Global
    internal static void RegisterGenericRepository(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped(typeof(IReadRepository<,>), typeof(GenericReadRepository<,>));
        services.AddScoped(typeof(IWriteRepository<,>), typeof(GenericWriteRepository<,>));
        services.AddScoped(typeof(IWriteRepository<,,>), typeof(GenericWriteRepository<,,>));
    }

    internal static void RegisterInterceptors(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IInterceptor, EvaluateAutoDatetimeColumnInterceptor>();
        services.AddScoped<IInterceptor, EntityChangesTriggerInterceptor>();
    }

    private static void RegisterRequiredServices(this IServiceCollection services)
    {
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
    }
}