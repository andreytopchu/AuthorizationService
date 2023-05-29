using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Identity.Domain.Specifications.User;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository;

public class UserReadRepository : GenericReadRepository<User, Guid>, IUserReadRepository
{
    private readonly IMapper _mapper;

    protected override IQueryable<User> BaseQuery => base.BaseQuery.Include(x => x.Role);

    public UserReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public async Task<TInfo> GetUserByIdAsync<TInfo>(Guid employeeId, CancellationToken cancellationToken)
    {
        var userInfo = await QueryBy(new UndeleteEntitySpecification<User>())
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (userInfo == null)
            throw new UserNotFoundException(employeeId);

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