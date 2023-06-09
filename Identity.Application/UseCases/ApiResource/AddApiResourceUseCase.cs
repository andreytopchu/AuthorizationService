using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Dal;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.ApiResource;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.ApiResource;

public class AddApiResourceUseCase : IUseCase<IAddApiResourceCommand, ApiResourceInfo>
{
    private readonly IdentityConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddApiResourceUseCase(IdentityConfigurationDbContext dbContext, IMapper mapper)
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

        return await GetApiResourceById(apiResource.Name, cancellationToken);
    }

    private async Task<ApiResourceInfo> GetApiResourceById(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.ApiResources.Where(new ApiResourceByNameSpecification(name))
            .ProjectTo<ApiResourceInfo>(_mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}