using System;

namespace Identity.Models.Requests.User;

public class BaseUserRequest
{
    public string Email { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public Guid RoleId { get; init; }
}