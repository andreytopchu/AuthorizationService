using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Abstractions;
using Identity.Application.Abstractions.Repositories.ApiResource;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.ApiResource;

public class ApiResourceReadRepository : GenericReadRepository<Domain.Entities.ApiResource, int>, IApiResourceReadRepository
{
    private readonly IMapper _mapper;

    public ApiResourceReadRepository(IReadDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    public async Task<TInfo> GetApiResourceById<TInfo>(int id, CancellationToken cancellationToken)
    {
        var apiResourceInfo = await QueryBy(new EntityByKeySpecification<Domain.Entities.ApiResource, int>(id))
            .ProjectTo<TInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (apiResourceInfo == null)
            throw new ApiResourceNotFoundException(id);

        return apiResourceInfo;
    }
}