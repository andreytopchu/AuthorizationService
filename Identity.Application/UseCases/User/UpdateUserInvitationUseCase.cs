using Identity.Application.Abstractions.Models.Command.User;
using Identity.Application.Abstractions.Models.Query.User;
using Identity.Application.Abstractions.Repositories.User;
using Identity.Application.Abstractions.UseCases;
using Identity.Application.Extensions;
using Identity.Domain.Exceptions;
using Identity.Domain.Specifications.User;

namespace Identity.Application.UseCases.User;

internal class UpdateUserInvitationUseCase : IUseCase<IUpdateUserInvitationCommand, UserInfo>
{
    private readonly IUserWriteRepository _userWriteRepository;
    private readonly IUseCase<ISendToUserActivationEmailCommand> _emailNotificationService;

    public UpdateUserInvitationUseCase(IUserWriteRepository userWriteRepository, IUseCase<ISendToUserActivationEmailCommand> emailNotificationService)
    {
        _userWriteRepository = userWriteRepository ?? throw new ArgumentNullException(nameof(userWriteRepository));
        _emailNotificationService = emailNotificationService ?? throw new ArgumentNullException(nameof(emailNotificationService));
    }

    public async Task<UserInfo> Process(IUpdateUserInvitationCommand arg, CancellationToken cancellationToken)
    {
        if (arg == null) throw new ArgumentNullException(nameof(arg));

        var userDb = await _userWriteRepository.Read.SingleOrDefaultAsync(new UndeletedUserByIdSpecification(arg.UserId), cancellationToken);

        if (userDb == null)
            throw new EntityNotFoundException<UserInfo>(arg.UserId);

        userDb.RoleId.ThrowIfRoleIdIsSuperAdmin();

        await _emailNotificationService.Process(new SendToUserActivationEmailRequest
        {
            User = userDb,
            EmailType = EmailType.RegisterUser
        }, cancellationToken);

        return await GetUserById(userDb.Id, cancellationToken);
    }

    private Task<UserInfo> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        return _userWriteRepository.Read.GetUserByIdAsync<UserInfo>(id, cancellationToken);
    }

    private class SendToUserActivationEmailRequest : ISendToUserActivationEmailCommand
    {
        public Domain.Entities.User User { get; init; } = null!;
        public EmailType EmailType { get; init; }
    }
}