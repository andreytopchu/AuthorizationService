namespace Identity.Domain.Entities;

public class ApiResourcePolicy
{
    public string PolicyName { get; set; }
    public string ResourceName { get; set; }
    public Guid PolicyId { get; set; }

    public Policy Policy { get; set; }
}