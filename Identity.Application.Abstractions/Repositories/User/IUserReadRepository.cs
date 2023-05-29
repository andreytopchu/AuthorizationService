using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.User;

public interface IUserReadRepository : IReadRepository<Domain.Entities.User, Guid>
{
    Task<TInfo> GetUserByIdAsync<TInfo>(Guid employeeId, CancellationToken cancellationToken);
    Task<Guid[]> GetUserIdsByRoleAsync(Guid roleId, CancellationToken cancellationToken);
    Task<Guid[]> GetUserIdsByPolicyAsync(Guid policyId, CancellationToken cancellationToken);
}