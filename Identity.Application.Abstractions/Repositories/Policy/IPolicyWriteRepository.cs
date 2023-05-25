using Identity.Abstractions.Repository;

namespace Identity.Application.Abstractions.Repositories.Policy;

public interface IPolicyWriteRepository : IWriteRepository<Domain.Entities.Policy, Guid, IPolicyReadRepository>
{
}