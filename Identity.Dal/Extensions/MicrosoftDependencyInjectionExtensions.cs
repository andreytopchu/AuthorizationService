using System;
using Dex.Cap.Outbox.AspNetScheduler;
using Dex.Cap.Outbox.Ef;
using Dex.Cap.Outbox.Interfaces;
using Identity.Abstractions;
using Identity.Abstractions.Repository;
using Identity.Application.Abstractions.Repositories.ApiResource;
using Identity.Application.Abstractions.Repositories.Policy;
using Identity.Application.Abstractions.Repositories.Role;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Dal.Interceptors;
using Identity.Dal.Repository;
using Identity.Dal.Repository.ApiResource;
using Identity.Dal.Repository.Policy;
using Identity.Dal.Repository.Role;
using Identity.Dal.Repository.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;

namespace Identity.Dal.Extensions;

public static class MicrosoftDependencyInjectionExtensions
{
    private const int DefaultRetryDelay = 500;

    public static void AddIdentityDal(this IServiceCollection services, string writeConnectionString, string? readConnectionString = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (writeConnectionString == null) throw new ArgumentNullException(nameof(writeConnectionString));

        // dbContexts
        services.RegisterDal<IdentityDbContext, IdentityReadDbContext>(writeConnectionString, readConnectionString);

        // repositories
        services.RegisterRepositories();

        // outbox
        services.AddOutbox<IdentityDbContext>();
        services.RegisterOutboxScheduler(10);
        services.AddScoped(typeof(IOutboxMessageHandler<>), typeof(GenericMassTransitPublisher<>));
        services.AddScoped<IOutboxService<IUnitOfWork>>(provider => provider.GetRequiredService<IOutboxService<IdentityDbContext>>());
    }

    private static void RegisterDal<TDbContext, TReadDbContext>(this IServiceCollection services, string writeConnectionString,
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
        services.AddScoped<IReadDbContext>(p => p.GetRequiredService<IdentityReadDbContext>());

        // default repositories
        services.RegisterGenericRepositories();
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
                    if (maxRetryCount > 0)
                    {
                        optionsBuilder.EnableRetryOnFailure(maxRetryCount,
                            maxRetryDelay != default ? maxRetryDelay : TimeSpan.FromMilliseconds(DefaultRetryDelay), null);
                    }
                });
        });
    }

    private static void RegisterGenericRepositories(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped(typeof(IReadRepository<,>), typeof(GenericReadRepository<,>))
            .AddScoped(typeof(IWriteRepository<,>), typeof(GenericWriteRepository<,>))
            .AddScoped(typeof(IWriteRepository<,,>), typeof(GenericWriteRepository<,,>));
    }

    private static void RegisterRepositories(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddScoped<IRoleReadRepository, RoleReadRepository>()
            .AddScoped<IRoleWriteRepository, RoleWriteRepository>()
            .AddScoped<IPolicyReadRepository, PolicyReadRepository>()
            .AddScoped<IPolicyWriteRepository, PolicyWriteRepository>()
            .AddScoped<IUserReadRepository, UserReadRepository>()
            .AddScoped<IUserWriteRepository, UserWriteRepository>()
            .AddScoped<IApiResourceReadRepository, ApiResourceReadRepository>();
    }

    private static void RegisterInterceptors(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<IInterceptor, EvaluateAutoDatetimeColumnInterceptor>();
        services.AddScoped<IInterceptor, EntityChangesTriggerInterceptor>();
    }

    private static void RegisterRequiredServices(this IServiceCollection services)
    {
        services.AddSingleton<ISystemClock, SystemClock>();
#pragma warning disable CS0618
        services.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
#pragma warning restore CS0618
    }
}