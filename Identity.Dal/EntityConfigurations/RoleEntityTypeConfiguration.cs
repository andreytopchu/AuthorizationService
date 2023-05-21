using System;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Dal.EntityConfigurations;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable(nameof(Role));

        builder.Property(e => e.Id);
        builder.Property(e => e.CreatedUtc);
        builder.Property(e => e.DeletedUtc);
        builder.Property(e => e.UpdatedUtc);
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Description);

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CreatedUtc).HasDatabaseName($"IX_{nameof(Role)}_{nameof(Role.CreatedUtc)}");
        builder.HasIndex(e => e.DeletedUtc).HasDatabaseName($"IX_{nameof(Role)}_{nameof(Role.DeletedUtc)}");
        builder.HasIndex(e => e.UpdatedUtc).HasDatabaseName($"IX_{nameof(Role)}_{nameof(Role.UpdatedUtc)}");
        builder.HasIndex(e => e.Name).HasDatabaseName($"IX_{nameof(Role)}_{nameof(Role.Name)}").IsUnique();

        builder.HasMany(e => e.Users).WithOne(e => e.Role).HasForeignKey(e => e.RoleId);
        builder.HasMany(e => e.Policies).WithMany(e => e.Roles);
    }
}