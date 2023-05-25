using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.Role;

public interface IRoleWriteRepository : IWriteRepository<Domain.Entities.Role, Guid, IRoleReadRepository>
{
}