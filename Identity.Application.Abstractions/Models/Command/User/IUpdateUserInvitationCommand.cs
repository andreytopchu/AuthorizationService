using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.User;

public interface IUpdateUserInvitationCommand : IUseCaseArg
{
    Guid UserId { get; }

    IIdentityUriCommand? IdentityUri { get; set; }
}