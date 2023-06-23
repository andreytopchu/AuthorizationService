using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.User;

namespace Identity.Application.UseCases.User;

internal class ResetPasswordUserUseCase : IUseCase<IResetPasswordUserCommand>
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly IUseCase<ISendToUserRecoveryEmailCommand> _emailNotificationService;

    public ResetPasswordUserUseCase(IUserReadRepository userReadRepository, IUseCase<ISendToUserRecoveryEmailCommand> emailNotificationService)
    {
        _userReadRepository = userReadRepository ?? throw new ArgumentNullException(nameof(userReadRepository));
        _emailNotificationService = emailNotificationService ?? throw new ArgumentNullException(nameof(emailNotificationService));
    }

    public async Task Process(IResetPasswordUserCommand arg, CancellationToken cancellationToken)
    {
        var dbUser = await _userReadRepository.SingleOrDefaultAsync(new ActiveUserByEmailSpecification(arg.Email), cancellationToken);

        if (dbUser == null) throw new EntityNotFoundException<UserInfo>(arg.Email);


        await _emailNotificationService.Process(new SendToUserRecoveryEmailRequest
        {
            User = dbUser,
            EmailType = EmailType.RecoveryPassword
        }, cancellationToken);
    }

    private class SendToUserRecoveryEmailRequest : ISendToUserRecoveryEmailCommand
    {
        public Domain.Entities.User User { get; init; } = null!;
        public EmailType EmailType { get; init; }
    }
}