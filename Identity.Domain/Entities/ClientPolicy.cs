using Dex.Entity;

namespace Identity.Domain.Entities;

public class ClientPolicy : IEntity<Guid>
{
    public Guid Id { get; set; }
    public string PolicyName { get; set; }
    public string ClientId { get; set; }
    public Guid PolicyId { get; set; }

    public Policy Policy { get; set; }
}