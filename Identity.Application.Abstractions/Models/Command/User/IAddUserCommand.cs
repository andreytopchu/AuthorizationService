using Identity.Application.Abstractions.UseCases;

namespace Identity.Application.Abstractions.Models.Command.User;

public interface IAddUserCommand : IUseCaseArg
{
    public string Password { get; }
    public string Email { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public Guid RoleId { get; init; }
    IIdentityUriCommand? IdentityUri { get; set; }
}