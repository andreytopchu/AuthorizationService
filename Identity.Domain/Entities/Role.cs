using Dex.Entity;

namespace Identity.Domain.Entities;

public sealed class Role : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime? DeletedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();

    public ICollection<Policy> Policies { get; set; } = new List<Policy>();
}