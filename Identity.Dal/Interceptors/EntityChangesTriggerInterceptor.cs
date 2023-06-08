using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dex.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Dal.Interceptors;

internal sealed class EntityChangesTriggerInterceptor : SaveChangesInterceptor
{
    private readonly IServiceProvider _serviceProvider;

    public EntityChangesTriggerInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        OnSavingChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        OnSavingChanges(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void OnSavingChanges(DbContext? dbContext)
    {
        if (dbContext == null) return;

        var allTriggers = _serviceProvider.GetServices<IEntityChangesTrigger>().ToArray();
        if (allTriggers.IsNullOrEmpty()) return;

        foreach (var entities in dbContext.ChangeTracker.Entries().GroupBy(x => x.Metadata.ClrType))
        {
            entities.GroupBy(x => x.State).ForEach(x =>
            {
                var entityState = GetEntityState(x);
                foreach (var trigger in allTriggers.Where(t => t.EntityType == entities.Key))
                {
                    trigger.RunTrigger(x.Select(ent => ent.Entity), entityState);
                }
            });
        }

        static Enum.EntityState GetEntityState(IGrouping<EntityState, EntityEntry> x)
        {
            var entityState = Enum.EntityState.None;
            var entityFrameworkModelState = x.Key;
            switch (entityFrameworkModelState)
            {
                case EntityState.Deleted:
                    entityState = Enum.EntityState.Deleted;
                    break;
                case EntityState.Modified:
                    entityState = Enum.EntityState.Modified;
                    break;
                case EntityState.Added:
                    entityState = Enum.EntityState.Added;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }

            return entityState;
        }
    }
}