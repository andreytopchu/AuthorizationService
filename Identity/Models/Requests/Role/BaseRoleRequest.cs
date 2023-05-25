using System;

namespace Identity.Models.Requests.Role;

public class BaseRoleRequest
{
    public string Name { get; init; }
    public Guid[] PolicyIds { get; init; }
    public string? Description { get; init; }
}