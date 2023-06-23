using Identity.Application.Abstractions.Models.Command.ApiResource;
using Identity.Application.Abstractions.UseCases;
using Identity.Dal;
using Identity.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.UseCases.ApiResource;

public class DeleteApiResourceUseCase : IUseCase<IDeleteApiResourceCommand>
{
    private readonly IdentityConfigurationDbContext _dbContext;

    public DeleteApiResourceUseCase(IdentityConfigurationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Process(IDeleteApiResourceCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var apiResource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Name == arg.ApiResourceName, cancellationToken);

        if (apiResource is null)
        {
            throw new ApiResourceNotFoundException(arg.ApiResourceName);
        }

        _dbContext.Remove(apiResource);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}