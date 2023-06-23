// ReSharper properties

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dex.Entity;
using Dex.Pagination;
using Dex.Specifications;
using Identity.Abstractions;
using Identity.Abstractions.Repository;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository;

public class GenericReadRepository<T, TK> : IReadRepository<T, TK>
    where T : class, IEntity<TK>, ICreatedUtc where TK : IComparable
{
    private readonly IMapper _mapper;
    private IReadDbContext DbContext { get; }
    protected virtual IQueryable<T> BaseQuery => DbContext.Get<T>();

    public GenericReadRepository(IReadDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        DbContext = dbContext;
    }

    public async Task<TInfo[]> GetWithPaginationAsync<TInfo>(IPaginationFilter paginationFilter, CancellationToken cancellationToken,
        params Expression<Func<T, object>>[] navigationProperties)
    {
        if (paginationFilter == null) throw new ArgumentNullException(nameof(paginationFilter));

        var baseQuery = navigationProperties.Aggregate(BaseQuery,
            (query, navigationProperty) => query.Include(navigationProperty));

        if (typeof(T).GetProperty(nameof(IDeletable.DeletedUtc)) != null)
        {
            baseQuery = baseQuery.Where(i => EF.Property<DateTime?>(i, nameof(IDeletable.DeletedUtc)) == null);
        }

        return await baseQuery.OrderByDescending(x => x.CreatedUtc)
            .FilterPage(paginationFilter.Page, paginationFilter.PageSize)
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<TInfo> GetByIdAsync<TInfo>(TK id, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties)
    {
        var baseQuery = QueryBy(new EntityByKeySpecification<T, TK>(id), navigationProperties);

        if (typeof(T).GetProperty(nameof(IDeletable.DeletedUtc)) != null)
        {
            baseQuery = baseQuery.Where(i => EF.Property<DateTime?>(i, nameof(IDeletable.DeletedUtc)) == null);
        }

        var result = await baseQuery.FirstOrDefaultAsync(cancellation);
        if (result == null)
            throw new EntityNotFoundException(id.ToString()!, typeof(T).Name);

        return _mapper.Map<TInfo>(result);
    }

    public async Task<T> GetByIdAsync(TK id, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties)
    {
        var result = await QueryBy(new EntityByKeySpecification<T, TK>(id), navigationProperties).FirstOrDefaultAsync(cancellation);
        if (result == null)
            throw new EntityNotFoundException(id.ToString()!, typeof(T).Name);

        return result;
    }

    public async Task<T> GetBySpecAsync(Specification<T> specification, CancellationToken cancellation,
        params Expression<Func<T, object>>[] navigationProperties)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        var result = await QueryBy(specification, navigationProperties).FirstOrDefaultAsync(specification, cancellation);
        if (result == null)
            throw new EntityNotFoundException<T>(specification.ToString());

        return result;
    }

    public Task<T[]> FilterAsync(Specification<T> specification, CancellationToken cancellation, params Expression<Func<T, object>>[] navigationProperties)
        => QueryBy(specification, navigationProperties).Where(specification).ToArrayAsync(cancellation);

    public Task<bool> AnyAsync(Specification<T> specification, CancellationToken cancellation)
        => BaseQuery.AnyAsync(specification, cancellation);

    public Task<T?> FirstOrDefaultAsync(Specification<T> specification, CancellationToken cancellation,
        params Expression<Func<T, object>>[] navigationProperties)
        => QueryBy(specification, navigationProperties).FirstOrDefaultAsync(specification, cancellation);

    public Task<T?> SingleOrDefaultAsync(Specification<T> specification, CancellationToken cancellation,
        params Expression<Func<T, object>>[] navigationProperties)
        => QueryBy(specification, navigationProperties).SingleOrDefaultAsync(specification, cancellation);

    // protected
    protected IQueryable<T> QueryBy(Specification<T> specification, params Expression<Func<T, object>>[] navigationProperties)
    {
        if (specification == null) throw new ArgumentNullException(nameof(specification));

        return navigationProperties.Aggregate(
            BaseQuery.Where(specification),
            (query, navigationProperty) => query.Include(navigationProperty));
    }
}