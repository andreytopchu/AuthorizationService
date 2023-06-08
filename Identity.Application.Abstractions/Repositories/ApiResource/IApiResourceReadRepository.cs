using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.ApiResource;

public interface IApiResourceReadRepository : IReadRepository<Domain.Entities.ApiResource, int>
{
    Task<TInfo> GetApiResourceById<TInfo>(int id, CancellationToken cancellationToken);
}