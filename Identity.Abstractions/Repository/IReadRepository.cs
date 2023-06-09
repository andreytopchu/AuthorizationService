using System.Linq.Expressions;
using Dex.Entity;
using Dex.Specifications;

namespace Identity.Abstractions.Repository;

public interface IReadRepository<T, in TK>
    where T : IEntity<TK>
    where TK : IComparable
{
    Task<T> GetByIdAsync(TK id, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);
    Task<TInfo> GetByIdAsync<TInfo>(TK id, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);

    Task<TInfo[]> GetWithPaginationAsync<TInfo>(IPaginationFilter paginationFilter, CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] navigationProperties);

    Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);
    Task<T[]> FilterAsync(Specification<T> specification, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);
    Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellation);
    Task<T?> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);
    Task<T?> SingleOrDefaultAsync(Specification<T> specification, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties);
}