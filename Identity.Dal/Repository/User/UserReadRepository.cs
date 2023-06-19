using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Identity.Domain.Specifications.User;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.User;

public class UserReadRepository : GenericReadRepository<Domain.Entities.User, Guid>, IUserReadRepository
{
    private readonly IMapper _mapper;

    protected override IQueryable<Domain.Entities.User> BaseQuery => base.BaseQuery.Include(x => x.Role);

    public UserReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public async Task<TInfo> GetUserByIdAsync<TInfo>(Guid userId, CancellationToken cancellationToken)
    {
        var userInfo = await QueryBy(new UndeleteEntitySpecification<Domain.Entities.User>())
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (userInfo == null)
            throw new UserNotFoundException(userId);

        return userInfo;
    }

    public async Task<Domain.Entities.User> GetActiveUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userInfo = await QueryBy(new ActiveUserByIdSpecification(userId))
            .Include(x => x.Role).ThenInclude(r => r != null ? r.Policies : null)
            .FirstOrDefaultAsync(cancellationToken);

        if (userInfo == null)
            throw new UserNotFoundException(userId);

        return userInfo;
    }

    public async Task<Guid[]> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await QueryBy(new ActiveUserByRoleIdSpecification(roleId))
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Guid[]> GetUserIdsByPolicyAsync(Guid policyId, CancellationToken cancellationToken)
    {
        return await QueryBy(new ActiveUserByPolicyIdSpecification(policyId))
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }
}