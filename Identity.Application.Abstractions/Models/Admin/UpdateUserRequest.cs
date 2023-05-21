namespace Identity.Application.Abstractions.Models.Admin;

public class UpdateUserRequest
{
    public string Email { get; }
    public string? FirstName { get; }
    public string? MiddleName { get; }
    public string? LastName { get; }
    public Guid RoleId { get; set; }
}