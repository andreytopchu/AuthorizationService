using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Query.ApiResource;

namespace Identity.Application.Abstractions.Repositories.ApiResource;

public interface IApiResourceReadRepository
{
    Task<ApiResourceInfo?> GetById(int id, CancellationToken cancellationToken);
    Task<ApiResourceInfo[]> Get(IPaginationFilter filter, CancellationToken cancellationToken);
}