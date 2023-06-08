using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.ApiResource;

public interface IApiResourceWriteRepository : IWriteRepository<Domain.Entities.ApiResource, int, IApiResourceReadRepository>
{
}