using System;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Dal.EntityConfigurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable(nameof(User));

        builder.Property(e => e.Id);
        builder.Property(e => e.CreatedUtc);
        builder.Property(e => e.DeletedUtc);
        builder.Property(e => e.UpdatedUtc);
        builder.Property(e => e.Phone).IsRequired();
        builder.Property(e => e.Email);
        builder.Property(e => e.EmailConfirmed);
        builder.Property(e => e.Password).IsRequired();

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CreatedUtc).HasDatabaseName($"IX_{nameof(User)}_{nameof(User.CreatedUtc)}");
        builder.HasIndex(e => e.DeletedUtc).HasDatabaseName($"IX_{nameof(User)}_{nameof(User.DeletedUtc)}");
        builder.HasIndex(e => e.UpdatedUtc).HasDatabaseName($"IX_{nameof(User)}_{nameof(User.UpdatedUtc)}");
        builder.HasIndex(e => e.Email).HasDatabaseName($"IX_{nameof(User)}_{nameof(User.Email)}").IsUnique();
        builder.HasIndex(e => e.Phone).HasDatabaseName($"IX_{nameof(User)}_{nameof(User.Phone)}").IsUnique();

        builder.HasOne(e => e.Role).WithMany(e => e.Users).HasForeignKey(e => e.RoleId);
    }
}