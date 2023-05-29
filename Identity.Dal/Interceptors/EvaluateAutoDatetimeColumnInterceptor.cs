// ReSharper properties

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dex.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Internal;

namespace Identity.Dal.Interceptors;

internal sealed class EvaluateAutoDatetimeColumnInterceptor : SaveChangesInterceptor
{
    private readonly ISystemClock _systemClock;

    public EvaluateAutoDatetimeColumnInterceptor(ISystemClock systemClock)
    {
        _systemClock = systemClock;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData?.Context != null)
        {
            EvaluateCreatedUpdatedDateTime(eventData.Context.ChangeTracker.Entries());
        }

        return base.SavingChanges(eventData!, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData?.Context != null)
        {
            EvaluateCreatedUpdatedDateTime(eventData.Context.ChangeTracker.Entries());
        }

        return base.SavingChangesAsync(eventData!, result, cancellationToken);
    }

    private void EvaluateCreatedUpdatedDateTime(IEnumerable<EntityEntry> changes)
    {
        var now = _systemClock.UtcNow;
        foreach (var x in changes)
        {
            if (x.State == EntityState.Added && x.Entity is ICreatedUtc created)
            {
                if (created.CreatedUtc == default)
                {
                    created.CreatedUtc = now.UtcDateTime;
                }
            }

            if (x.State is EntityState.Added or EntityState.Modified && x.Entity is IUpdatedUtc updated)
            {
                updated.UpdatedUtc = now.UtcDateTime;
            }
        }
    }
}