using Identity.Abstractions.Repository;
using Identity.Application.Abstractions.Models.Query.Policy;

namespace Identity.Application.Abstractions.Repositories.Policy;

public interface IPolicyReadRepository : IReadRepository<Domain.Entities.Policy, Guid>
{
    Task<bool> IsPolicyExistAsync(Guid id, CancellationToken cancellationToken);
    Task<TInfo> GetPolicyByIdAsync<TInfo>(Guid roleId, CancellationToken cancellationToken);
    Task<PolicyInfo[]> GetPolicies(CancellationToken cancellationToken);
}