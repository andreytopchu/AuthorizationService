// ReSharper properties

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dex.Entity;
using Dex.Specifications;
using Identity.Abstractions;
using Identity.Abstractions.Repository;
using Identity.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository;

public class GenericReadRepository<T, TK> : IReadRepository<T, TK>
    where T : class, IEntity<TK> where TK : IComparable
{
    private IReadDbContext DbContext { get; }
    protected virtual IQueryable<T> BaseQuery => DbContext.Get<T>();

    public GenericReadRepository(IReadDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<T> GetByIdAsync(TK id, CancellationToken cancellation)
    {
        var result = await BaseQuery.FirstOrDefaultAsync(x => Equals(x.Id, id), cancellation);
        if (result == null)
            throw new EntityNotFoundException(id.ToString()!, typeof(T).Name);

        return result;
    }

    public async Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellation)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var result = await BaseQuery.FirstOrDefaultAsync(specification, cancellation);
        if (result == null)
            throw new EntityNotFoundException<T>(specification.ToString());

        return result;
    }

    public Task<T[]> FilterAsync(Specification<T> specification, CancellationToken cancellation)
        => BaseQuery.Where(specification).ToArrayAsync(cancellation);

    public Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellation)
        => BaseQuery.AnyAsync(specification, cancellation);

    public Task<T?> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellation)
        => BaseQuery.FirstOrDefaultAsync(specification, cancellation);

    public Task<T?> SingleOrDefaultAsync(Specification<T> specification, CancellationToken cancellation)
        => BaseQuery.SingleOrDefaultAsync(specification, cancellation);

    // protected
    protected IQueryable<T> QueryBy(Specification<T> specification) => BaseQuery.Where(specification);
}