using System;
using Identity.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal;

internal class IdentityReadDbContext : BaseDbContext<IdentityReadDbContext>
{
    protected override bool IsReadOnly => true;

    public IdentityReadDbContext(DbContextOptions<IdentityReadDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));

        //entityConfigurations
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ServicePolicyEntityTypeConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}