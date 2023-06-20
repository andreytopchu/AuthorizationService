using System;
using Dex.Cap.Outbox.Ef;
using Identity.Dal.EntityConfigurations;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal;

public class IdentityDbContext : BaseDbContext<IdentityDbContext>
{
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Policy> Policies => Set<Policy>();

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
        modelBuilder.ApplyConfiguration(new ApiResourcePolicyEntityTypeConfiguration());

        modelBuilder.OutboxModelCreating();
        base.OnModelCreating(modelBuilder);
    }
}