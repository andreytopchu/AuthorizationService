using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.Models.Query.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.ApiResource;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.ApiResource;

public class UpdateApiResourceUseCase : IUseCase<IUpdateApiResourceCommand, ApiResourceInfo>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateApiResourceUseCase(ConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApiResourceInfo> Process(IUpdateApiResourceCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var apiResource = await _dbContext.ApiResources.Where(new ApiResourceByIdSpecification(arg.Id)).FirstOrDefaultAsync(cancellationToken);

        if (apiResource is null)
        {
            throw new ApiResourceNotFoundException(arg.Id);
        }

        //update secrets
        var apiResourceSecrets = apiResource.Secrets.Where(x => arg.ApiSecrets.Contains(x.Value)).ToList();
        apiResourceSecrets.AddRange(arg.ApiSecrets.Except(apiResourceSecrets.Select(rs => rs.Value)).Select(x => new ApiResourceSecret {Value = x}));
        apiResource.Secrets = apiResourceSecrets;

        //update grant types
        var userClaims = apiResource.UserClaims.Where(x => arg.UserClaims.Contains(x.Type)).ToList();
        userClaims.AddRange(arg.UserClaims.Except(userClaims.Select(uc => uc.Type)).Select(x => new ApiResourceClaim {Type = x}));
        apiResource.UserClaims = userClaims;

        //update scopes
        var scopes = apiResource.Scopes.Where(x => arg.Scopes.Contains(x.Scope)).ToList();
        scopes.AddRange(arg.Scopes.Except(scopes.Select(s => s.Scope)).Select(x => new ApiResourceScope {Scope = x}));
        apiResource.Scopes = scopes;

        _mapper.Map(arg, apiResource);

        _dbContext.Update(apiResource);
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