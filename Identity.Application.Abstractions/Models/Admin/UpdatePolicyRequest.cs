namespace Identity.Application.Abstractions.Models.Admin;

public class UpdatePolicyRequest
{
    public Guid Id { get; init; }
    public string PolicyName { get; init; }
    public string ClientId { get; init; }
}