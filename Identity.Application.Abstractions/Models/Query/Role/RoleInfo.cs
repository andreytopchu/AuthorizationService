namespace Identity.Application.Abstractions.Models.Query.Role;

public class RoleInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public PolicyDto[] Policies { get; init; }
}