using Dex.Ef.Contracts.Entities;

namespace Identity.Domain.Entities;

public class Policy : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime? DeletedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public string Name { get; set; } = null!;

    public ICollection<Role> Roles { get; set; } = Array.Empty<Role>();
}