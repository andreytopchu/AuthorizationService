// ReSharper properties

using Dex.Entity;

namespace Identity.Abstractions.Repository;

public interface IWriteRepository<T, in TK>
    where T : IEntity<TK>
    where TK : IComparable
{
    IReadRepository<T, TK> Read { get; }

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> RemoveAsync(TK key, CancellationToken cancellationToken = default);
}

public interface IWriteRepository<T, in TK, out TReadRepository>
    where TReadRepository : IReadRepository<T, TK>
    where T : IEntity<TK>
    where TK : IComparable
{
    TReadRepository Read { get; }

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> RemoveAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> RemoveAsync(TK key, CancellationToken cancellationToken = default);
}