using Identity.Application.Abstractions.Models.Command.User;

namespace Identity.Models.Requests.User;

public class AddUserRequest : BaseUserRequest, IAddUserCommand
{
    public string Password { get; }
}