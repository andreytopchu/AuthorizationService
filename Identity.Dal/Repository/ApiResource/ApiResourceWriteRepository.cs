using AutoMapper;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.ApiResource;

namespace Identity.Dal.Repository.ApiResource;

public class ApiResourceWriteRepository : GenericWriteRepository<Domain.Entities.ApiResource, int, IApiResourceReadRepository>, IApiResourceWriteRepository
{
    public ApiResourceWriteRepository(IWriteDbContext writeDbContext, IMapper mapper)
        : base(writeDbContext, new ApiResourceReadRepository(writeDbContext, mapper))
    {
    }
}