using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.Dal;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Client;
using IdentityModel;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.Client;

public class UpdateClientUseCase : IUseCase<IUpdateClientCommand, ClientInfo>
{
    private readonly IdentityConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UpdateClientUseCase(IdentityConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientInfo> Process(IUpdateClientCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var clientDb = await _dbContext.Clients
            .Where(new ClientByClientIdSpecification(arg.ClientId))
            .Include(x => x.ClientSecrets)
            .Include(x => x.AllowedGrantTypes)
            .Include(x => x.AllowedScopes)
            .Include(x => x.Claims)
            .FirstOrDefaultAsync(cancellationToken);
        if (clientDb is null)
        {
            throw new ClientNotFoundException(arg.ClientId);
        }

        //update secrets
        var clientSecrets = clientDb.ClientSecrets.Where(x => arg.ApiSecrets.Contains(x.Value)).ToList();
        clientSecrets.AddRange(arg.ApiSecrets.Except(clientSecrets.Select(cs => cs.Value)).Select(x => new ClientSecret {Value = x.ToSha256()}));
        clientDb.ClientSecrets = clientSecrets;

        //update grant types
        var grandTypes = clientDb.AllowedGrantTypes.Where(x => arg.AllowedGrantTypes.Contains(x.GrantType)).ToList();
        grandTypes.AddRange(arg.AllowedGrantTypes.Except(grandTypes.Select(gt => gt.GrantType)).Select(x => new ClientGrantType {GrantType = x}));
        clientDb.AllowedGrantTypes = grandTypes;

        //update allowed scopes
        var allowedScopes = clientDb.AllowedScopes.Where(x => arg.AllowedScopes.Contains(x.Scope)).ToList();
        allowedScopes.AddRange(arg.AllowedScopes.Except(allowedScopes.Select(s => s.Scope)).Select(x => new ClientScope {Scope = x}));
        clientDb.AllowedScopes = allowedScopes;

        //update user claims
        var userClaims = clientDb.Claims.Where(x => arg.UserClaims.Contains(x.Value)).ToList();
        userClaims.AddRange(arg.UserClaims.Except(userClaims.Select(s => s.Value)).Select(x => new ClientClaim {Value = x}));
        clientDb.Claims = userClaims;

        _mapper.Map(arg, clientDb);

        _dbContext.Update(clientDb);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetClientById(clientDb.ClientId, cancellationToken);
    }

    private async Task<ClientInfo> GetClientById(string clientId, CancellationToken cancellationToken)
    {
        return await _dbContext.Clients.Where(new ClientByClientIdSpecification(clientId))
            .ProjectTo<ClientInfo>(_mapper.ConfigurationProvider).FirstAsync(cancellationToken);
    }
}