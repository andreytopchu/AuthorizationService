using Dex.Ef.Contracts.Entities;

namespace Identity.Domain.Entities;

public sealed class Role : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime? DeletedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = Array.Empty<User>();

    public ICollection<Policy> Policies { get; set; } = Array.Empty<Policy>();
}