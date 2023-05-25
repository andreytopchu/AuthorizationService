using System;
using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User;

public class UpdateUserRequest : BaseUserRequest, IUpdateUserCommand
{
    public Guid Id { get; init; }
}