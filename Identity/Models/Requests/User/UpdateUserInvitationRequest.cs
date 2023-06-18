using System;
using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User;

public class UpdateUserInvitationRequest : IUpdateUserInvitationCommand
{
    public Guid UserId { get; init; }
    public IIdentityUriCommand? IdentityUri { get; set; }
}