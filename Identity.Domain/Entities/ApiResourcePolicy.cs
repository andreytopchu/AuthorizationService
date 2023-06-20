using Dex.Entity;

namespace Identity.Domain.Entities;

public class ApiResourcePolicy : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string PolicyName { get; set; }
    public string ResourceName { get; set; }
    public Guid PolicyId { get; set; }

    public Policy Policy { get; set; }
}