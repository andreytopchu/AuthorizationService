using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.ApiResource;

public class DeleteApiResourceUseCase : IUseCase<IDeleteApiResourceCommand>
{
    private readonly ConfigurationDbContext _dbContext;

    public DeleteApiResourceUseCase(ConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Process(IDeleteApiResourceCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var apiResource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == arg.ApiResourceId, cancellationToken);

        if (apiResource is null)
        {
            throw new ApiResourceNotFoundException(arg.ApiResourceId);
        }

        _dbContext.Remove(apiResource);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}