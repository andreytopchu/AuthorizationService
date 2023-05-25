using System.Transactions;

namespace Identity.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task ExecuteInTransaction(IsolationLevel isolationLevel, Func<CancellationToken, Task> action, CancellationToken cancellationToken);
}