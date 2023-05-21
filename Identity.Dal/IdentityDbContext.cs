using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Identity.Application.Abstractions.Exceptions;
using Identity.Dal.EntityConfigurations;
using Identity.Dal.Extensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Identity.Dal;

public class IdentityDbContext : DbContext
{
    public Task ExecuteInTransaction(IsolationLevel isolationLevel, Func<CancellationToken, Task> action,
        CancellationToken cancellationToken)
    {
        return Database.CreateExecutionStrategy()
            .ExecuteInTransactionScopeAsync(action, TransactionScopeOption.Required, isolationLevel, cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            if (e.InnerException is PostgresException
                {
                    IsTransient: false, SqlState: PostgresErrorCodes.UniqueViolation
                } pge)
            {
                throw new EntityAlreadyExistsException(pge.MessageText, e);
            }

            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

        //entityConfigurations
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);

        // register extensions
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // register advanced
        modelBuilder.NormalizeEmail();

        // установка для всех моделей конкаренси токен
        modelBuilder.UseXminAsConcurrencyToken(this);
        modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
    }
}