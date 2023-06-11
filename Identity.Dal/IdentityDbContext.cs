using System;
using Dex.Cap.Outbox.Ef;
using Identity.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal;

public class IdentityDbContext : BaseDbContext<IdentityDbContext>
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

        //entityConfigurations
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PolicyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ClientPolicyEntityTypeConfiguration());

        modelBuilder.OutboxModelCreating();
        base.OnModelCreating(modelBuilder);
    }
}