using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.Dal;
using Identity.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.Client;

public class DeleteClientUseCase : IUseCase<IDeleteClientCommand>
{
    private readonly IdentityConfigurationDbContext _dbContext;

    public DeleteClientUseCase(IdentityConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Process(IDeleteClientCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var client = await _dbContext.Clients.FirstOrDefaultAsync(x => x.ClientId == arg.ClientId, cancellationToken);

        if (client is null)
        {
            throw new ClientNotFoundException(arg.ClientId);
        }

        _dbContext.Remove(client);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}