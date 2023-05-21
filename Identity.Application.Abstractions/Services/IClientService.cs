using Identity.Application.Abstractions.Models.Admin;

namespace Identity.Application.Abstractions.Services;

public interface IClientService
{
    Task AddClient(AddClientRequest request, CancellationToken cancellationToken);
    Task UpdateClient(UpdateClientRequest request, CancellationToken cancellationToken);
    Task DeleteClient(string clientId, CancellationToken cancellationToken);
}