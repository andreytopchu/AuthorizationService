using Identity.Application.Abstractions.Models.Query.ClientPolicy;

namespace Identity.Application.Abstractions.Models.Query.Policy;

public class PolicyInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public ApiResourcePolicyInfo[] ApiResourcePolicyInfos { get; set; } = Array.Empty<ApiResourcePolicyInfo>();
}