using Dex.Entity;

namespace Identity.Domain.Entities;

public sealed class User : ICreatedUtc, IUpdatedUtc, IDeletable, IEntity<Guid>
{
    public Guid Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? DeletedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public Guid RoleId { get; set; }
    public string? Phone { get; set; }
    public string Email { get; set; } = null!;
    public DateTime? EmailConfirmed { get; set; }
    public string Password { get; set; } = null!;
    public Role Role { get; set; }

    public string GetFullName()
    {
        return string.Join(' ', LastName, FirstName, MiddleName);
    }
}