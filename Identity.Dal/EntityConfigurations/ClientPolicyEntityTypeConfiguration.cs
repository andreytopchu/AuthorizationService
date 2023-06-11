using System;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Dal.EntityConfigurations;

public class ClientPolicyEntityTypeConfiguration : IEntityTypeConfiguration<ClientPolicy>
{
    public void Configure(EntityTypeBuilder<ClientPolicy> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable(nameof(ClientPolicy));

        builder.Property(e => e.Id);
        builder.Property(e => e.PolicyName);
        builder.Property(e => e.ClientId);
        builder.Property(e => e.PolicyId);

        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Policy).WithMany(e => e.Clients).HasForeignKey(x=>x.PolicyId);
    }
}