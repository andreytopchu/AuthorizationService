using Identity.Application.Abstractions.Models.Admin;

namespace Identity.Application.Abstractions.Services;

public interface IUserService
{
    Task AddUser(AddUserRequest request, CancellationToken cancellationToken);
    Task UpdateUser(UpdateUserRequest request, CancellationToken cancellationToken);
    Task DeleteUser(Guid userId, CancellationToken cancellationToken);
}