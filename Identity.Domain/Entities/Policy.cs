using Dex.Entity;

namespace Identity.Domain.Entities;

public class Policy : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime? DeletedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<ApiResourcePolicy> ApiResources { get; set; } = new List<ApiResourcePolicy>();
}