using Identity.Application.Abstractions.Models.Query.ClientPolicy;

namespace Identity.Application.Abstractions.Models.Query.Policy;

public class PolicyInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public ClientPolicyInfo[] ClientPolicyInfos { get; set; } = Array.Empty<ClientPolicyInfo>();
}