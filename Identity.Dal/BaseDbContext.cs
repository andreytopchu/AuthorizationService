using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Identity.Abstractions;
using Identity.Dal.Extensions;
using Identity.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Identity.Dal
{
    public abstract class BaseDbContext<T> : DbContext, IWriteDbContext, IUnitOfWork
        where T : DbContext
    {
        protected virtual bool IsReadOnly => ChangeTracker.QueryTrackingBehavior == QueryTrackingBehavior.NoTracking;

        public IQueryable<TEntity> Get<TEntity>() where TEntity : class => Set<TEntity>();

        protected BaseDbContext(DbContextOptions<T> options)
            : base(options)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            if (IsReadOnly && ChangeTracker.QueryTrackingBehavior != QueryTrackingBehavior.NoTracking)
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }

            SavingChanges += ThrowIfReadOnly;
        }

        public new object Add(object entity) => base.Add(entity).Entity;
        public new object Update(object entity) => base.Update(entity).Entity;
        public new object Remove(object entity) => base.Remove(entity).Entity;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                if (e.InnerException is PostgresException {IsTransient: false, SqlState: PostgresErrorCodes.UniqueViolation} pge)
                {
                    throw new EntityAlreadyExistsException(pge.MessageText, e);
                }
                else
                {
                    throw;
                }
            }
        }

        public Task ExecuteInTransaction(IsolationLevel isolationLevel, Func<CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            return Database.CreateExecutionStrategy()
                .ExecuteInTransactionScopeAsync(action, TransactionScopeOption.Required, isolationLevel, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);

            // register extensions
            modelBuilder.HasPostgresExtension("uuid-ossp");

            // register advanced
            modelBuilder.NormalizeEmail();
            // установка для всех моделей конкаренси токен
            modelBuilder.UseXminAsConcurrencyToken(this, GetIgnoreConcurrencyTokenTypes());
            modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        }

        private static Type[] GetIgnoreConcurrencyTokenTypes()
        {
            return Array.Empty<Type>();
        }

        private void ThrowIfReadOnly(object? sender, SavingChangesEventArgs e)
        {
            if (IsReadOnly) throw new NotSupportedException("context is readonly");
        }
    }
}