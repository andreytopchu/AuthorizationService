using System;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Dal.EntityConfigurations;

public class PolicyEntityTypeConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable(nameof(Policy));

        builder.Property(e => e.Id);
        builder.Property(e => e.CreatedUtc);
        builder.Property(e => e.DeletedUtc);
        builder.Property(e => e.UpdatedUtc);
        builder.Property(e => e.Name).IsRequired();

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CreatedUtc).HasDatabaseName($"IX_{nameof(Policy)}_{nameof(Policy.CreatedUtc)}");
        builder.HasIndex(e => e.DeletedUtc).HasDatabaseName($"IX_{nameof(Policy)}_{nameof(Policy.DeletedUtc)}");
        builder.HasIndex(e => e.UpdatedUtc).HasDatabaseName($"IX_{nameof(Policy)}_{nameof(Policy.UpdatedUtc)}");
        builder.HasIndex(e => e.Name).HasDatabaseName($"IX_{nameof(Policy)}_{nameof(Policy.Name)}").IsUnique();

        builder.HasMany(e => e.Roles).WithMany(e => e.Policies);
    }
}