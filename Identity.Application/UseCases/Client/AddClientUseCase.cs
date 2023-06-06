using AutoMapper;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Client;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.Client;

public class AddClientUseCase : IUseCase<IAddClientCommand, ClientInfo>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddClientUseCase(ConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientInfo> Process(IAddClientCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        if (_dbContext.Clients.Where(new ClientByClientIdSpecification(arg.ClientId)).Any())
        {
            throw new EntityAlreadyExistsException<Domain.Entities.Client>(arg.ClientId);
        }

        var client = _mapper.Map<Domain.Entities.Client>(arg);

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetClientById(client.ClientId, cancellationToken);
    }

    private async Task<ClientInfo> GetClientById(string clientId, CancellationToken cancellationToken)
    {
        var client = await _dbContext.Clients.Where(new ClientByClientIdSpecification(clientId)).FirstAsync(cancellationToken);

        return new ClientInfo
        {
            ClientId = client.ClientId,
            ClientName = client.ClientName,
            ApiSecrets = client.ClientSecrets.Select(x => x.Value).ToArray(),
            AccessTokenType = (AccessTokenType) client.AccessTokenType,
            AllowedGrantTypes = client.AllowedGrantTypes.Select(x => x.GrantType).ToArray(),
            AllowOfflineAccess = client.AllowOfflineAccess,
            UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh,
            RefreshTokenUsage = (TokenUsage) client.RefreshTokenUsage,
            RefreshTokenExpiration = (TokenExpiration) client.RefreshTokenExpiration,
            AccessTokenLifetime = client.AccessTokenLifetime,
            AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime,
            SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime,
            IsEnabled = client.Enabled
        };
    }
}