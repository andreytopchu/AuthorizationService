using System;
using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User;

public class DeleteUserRequest : IDeleteUserCommand
{
    public Guid UserId { get; init; }
}