using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Query.Client;

namespace Identity.Application.Abstractions.Repositories.Client;

public interface IClientReadRepository
{
    Task<ClientInfo?> GetById(string clientId, CancellationToken cancellationToken);
    Task<ClientInfo[]> Get(IPaginationFilter filter, CancellationToken cancellationToken);
}