namespace Identity.Application.Abstractions.Models.Admin;

public class UpdateRoleRequest
{
    public Guid Id { get; init; }
    public string RoleName { get; init; }
    public Guid[] PolicyIds { get; init; }
}