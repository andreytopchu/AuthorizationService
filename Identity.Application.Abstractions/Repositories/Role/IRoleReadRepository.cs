using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.Role;

public interface IRoleReadRepository : IReadRepository<Domain.Entities.Role, Guid>
{
    Task<bool> IsRoleExistAsync(Guid id, CancellationToken cancellationToken);
    Task<TInfo> GetRoleByIdAsync<TInfo>(Guid roleId, CancellationToken cancellationToken);
}