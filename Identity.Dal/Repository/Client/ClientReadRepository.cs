using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Dex.Pagination;
using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.Repositories.Client;
using Identity.Domain.Specifications.Client;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal.Repository.Client;

public class ClientReadRepository : IClientReadRepository
{
    private readonly IdentityConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public ClientReadRepository(IdentityConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientInfo?> GetById(string clientId, CancellationToken cancellationToken)
    {
        return await _dbContext.Clients
            .Where(new ClientByClientIdSpecification(clientId))
            .ProjectTo<ClientInfo>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ClientInfo[]> Get(IPaginationFilter filter, CancellationToken cancellationToken)
    {
        return await _dbContext.Clients
            .OrderByDescending(x => x.Created)
            .FilterPage(filter.Page, filter.PageSize)
            .ProjectTo<ClientInfo>(_mapper.ConfigurationProvider)
            .ToArrayAsync(cancellationToken);
    }
}