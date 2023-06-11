using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.ApiResource;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.ApiResource;

public class AddApiResourceUseCase : IUseCase<IAddApiResourceCommand, ApiResourceInfo>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddApiResourceUseCase(ConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResourceInfo> Process(IAddApiResourceCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        if (await _dbContext.ApiResources.Where(new ApiResourceByNameSpecification(arg.Name)).SingleOrDefaultAsync(cancellationToken) is not null)
        {
            throw new EntityAlreadyExistsException<IdentityServer4.EntityFramework.Entities.ApiResource>(arg.Name);
        }

        var apiResource = _mapper.Map<IdentityServer4.EntityFramework.Entities.ApiResource>(arg);

        await _dbContext.AddAsync(apiResource, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetApiResourceById(apiResource.Id, cancellationToken);
    }

    private async Task<ApiResourceInfo> GetApiResourceById(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.ApiResources.Where(new ApiResourceByIdSpecification(id))
            .ProjectTo<ApiResourceInfo>(_mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}