// ReSharper properties

using System;
using System.Linq;
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

    public async Task<TInfo[]> GetWithPaginationAsync<TInfo>(IPaginationFilter paginationFilter, CancellationToken cancellationToken)
    {
        if (paginationFilter == null) throw new ArgumentNullException(nameof(paginationFilter));

        return await BaseQuery.OrderByDescending(x => x.CreatedUtc)
            .FilterPage(paginationFilter.Page, paginationFilter.PageSize)
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<TInfo> GetByIdAsync<TInfo>(TK id, CancellationToken cancellation)
    {
        var result = await BaseQuery.FirstOrDefaultAsync(x => Equals(x.Id, id), cancellation);
        if (result == null)
            throw new EntityNotFoundException(id.ToString()!, typeof(T).Name);

        return _mapper.Map<TInfo>(result);
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