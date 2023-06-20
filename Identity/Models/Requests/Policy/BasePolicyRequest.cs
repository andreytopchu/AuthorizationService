namespace Identity.Models.Requests.Policy;

public class BasePolicyRequest
{
    public string Name { get; init; }
    public string[] ResourceNames { get; init; }
}