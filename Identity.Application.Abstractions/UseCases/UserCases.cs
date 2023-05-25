using Identity.Application.Abstractions.Services;

namespace Identity.Application.Abstractions.UseCases;

public abstract class BaseUserCase : IUseCase
{
    protected Guid UserId => UserIdService.UserId;
    private IUserIdService UserIdService { get; }

    protected BaseUserCase(IUserIdService userIdService)
    {
        UserIdService = userIdService ?? throw new ArgumentNullException(nameof(userIdService));
    }
}