using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.User;

public interface IUserWriteRepository : IWriteRepository<Domain.Entities.User, Guid, IUserReadRepository>
{
}