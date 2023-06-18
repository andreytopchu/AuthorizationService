using System;
using Identity.Dal.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal;

public class IdentityConfigurationDbContext : ConfigurationDbContext<IdentityConfigurationDbContext>
{
    public IdentityConfigurationDbContext(DbContextOptions<IdentityConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options,
        storeOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

        modelBuilder.SetDefaultDateTimeKind(DateTimeKind.Utc);
        base.OnModelCreating(modelBuilder);
    }
}