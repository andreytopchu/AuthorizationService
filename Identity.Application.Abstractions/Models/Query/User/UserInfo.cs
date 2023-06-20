namespace Identity.Application.Abstractions.Models.Query.User;

public class UserInfo
{
    public Guid Id { get; init; }
    public DateTime CreatedUtc { get; init; }
    public string Phone { get; init; }
    public string Email { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public RoleDto Role { get; init; }
    public bool IsActive { get; init; }
}