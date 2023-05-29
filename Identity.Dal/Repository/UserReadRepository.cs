using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Domain.Entities;
using Identity.Domain.Specifications.User;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository;

public class UserReadRepository : GenericReadRepository<User, Guid>, IUserReadRepository
{
    public UserReadRepository(IReadDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<TInfo> GetUserByIdAsync<TInfo>(Guid employeeId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid[]> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid[]> GetUserIdsByPolicyAsync(Guid policyId, CancellationToken cancellationToken)
    {
        return await QueryBy(new ActiveUserByPolicyIdSpecification(policyId)).Select(x => x.Id).ToArrayAsync(cancellationToken);
    }
}