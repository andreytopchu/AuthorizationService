using AutoMapper;
using AutoMapper.QueryableExtensions;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.Models.Query.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.Dal;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.Client;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.Client;

public class AddClientUseCase : IUseCase<IAddClientCommand, ClientInfo>
{
    private readonly IdentityConfigurationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AddClientUseCase(IdentityConfigurationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ClientInfo> Process(IAddClientCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        if (_dbContext.Clients.Where(new ClientByClientIdSpecification(arg.ClientId)).Any())
        {
            throw new EntityAlreadyExistsException<IdentityServer4.EntityFramework.Entities.Client>(arg.ClientId);
        }

        var client = _mapper.Map<IdentityServer4.EntityFramework.Entities.Client>(arg);

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetClientById(client.ClientId, cancellationToken);
    }

    private async Task<ClientInfo> GetClientById(string clientId, CancellationToken cancellationToken)
    {
        return await _dbContext.Clients.Where(new ClientByClientIdSpecification(clientId))
            .ProjectTo<ClientInfo>(_mapper.ConfigurationProvider).FirstAsync(cancellationToken);
    }
}