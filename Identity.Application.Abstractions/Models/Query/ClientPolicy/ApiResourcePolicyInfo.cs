namespace Identity.Application.Abstractions.Models.Query.ClientPolicy;

public class ApiResourcePolicyInfo
{
    public Guid Id { get; set; }
    public string PolicyName { get; set; } = string.Empty;
}