using Identity.Abstractions;
using Identity.Application.Abstractions.Models.Command.Client;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.Client;

public class DeleteClientUseCase : IUseCase<IDeleteClientCommand>
{
    private readonly ConfigurationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClientUseCase(ConfigurationDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}