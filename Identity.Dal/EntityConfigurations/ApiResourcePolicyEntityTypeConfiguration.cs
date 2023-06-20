using System;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Dal.EntityConfigurations;

public class ApiResourcePolicyEntityTypeConfiguration : IEntityTypeConfiguration<ApiResourcePolicy>
{
    public void Configure(EntityTypeBuilder<ApiResourcePolicy> builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.ToTable(nameof(ApiResourcePolicy));

        builder.Property(e => e.Id);
        builder.Property(e => e.PolicyName);
        builder.Property(e => e.ResourceName);
        builder.Property(e => e.PolicyId);

        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Policy).WithMany(e => e.ApiResources).HasForeignKey(x=>x.PolicyId);
    }
}