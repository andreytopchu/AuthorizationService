using Identity.Application.Abstractions.Models.Admin;

namespace Identity.Application.Abstractions.Services;

public interface IRoleService
{
    Task AddRole(AddRoleRequest request, CancellationToken cancellationToken);
    Task UpdateRole(UpdateRoleRequest request, CancellationToken cancellationToken);
    Task DeleteRole(Guid roleId, CancellationToken cancellationToken);
}