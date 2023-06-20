using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dex.Pagination;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.Repositories.ApiResource;
using Identity.Domain.Specifications.ApiResource;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.ApiResource;

public class ApiResourceReadRepository : IApiResourceReadRepository
{
    private readonly IdentityConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ApiResourceReadRepository(IdentityConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResourceInfo?> GetById(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.ApiResources
            .Where(new ApiResourceByIdSpecification(id))
            .ProjectTo<ApiResourceInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApiResourceInfo[]> Get(IPaginationFilter filter, CancellationToken cancellationToken)
    {
        return await _dbContext.ApiResources
            .OrderByDescending(x=>x.Created)
            .FilterPage(filter.Page, filter.PageSize)
            .ProjectTo<ApiResourceInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}