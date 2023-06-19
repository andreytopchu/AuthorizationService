using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Extensions;
using Identity.Application.Abstractions.Services;
using Identity.Dal;
using Identity.Domain.Constants;
using Identity.Domain.Entities;
using Identity.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;

namespace IdentityDbSeeder.Seeder;

public class IdentityDbSeeder : BaseEfSeeder<IdentityDbContext>, IDbSeeder
{
    private readonly IHostEnvironment _environment;
    private readonly ISystemClock _systemClock;
    private readonly IPasswordHashGenerator _passwordHashGenerator;

    public IdentityDbSeeder(IHostEnvironment environment, ISystemClock systemClock,
        IPasswordHashGenerator passwordHashGenerator, IdentityDbContext dbContext) : base(dbContext)
    {
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _passwordHashGenerator = passwordHashGenerator ?? throw new ArgumentNullException(nameof(passwordHashGenerator));
    }

    protected override async Task EnsureSeedData()
    {
        if (DbContext == null) throw new ArgumentNullException(nameof(DbContext));

        if (_environment.IsProduction())
        {
            await ProductionSeed(DbContext);
        }
        else
        {
            await DevelopmentSeed(DbContext);
        }

        await DbContext.SaveChangesAsync();
    }

    private async Task ProductionSeed(IdentityDbContext dbContext)
    {
        if (!await dbContext.Policies.AnyAsync(x => x.Id == PolicyConstantId.FullAccessPolicyId))
        {
            await dbContext.Policies.AddAsync(new Policy
            {
                Id = PolicyConstantId.FullAccessPolicyId,
                Name = "fullAccess",
                Clients = new ClientPolicy[]
                {
                    new() {ClientId = ClientConstantId.IdentityClientId, PolicyId = PolicyConstantId.FullAccessPolicyId, PolicyName = "fullAccess"}
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        if (!await dbContext.Roles.AnyAsync(x => x.Id == RoleConstantId.SuperAdminRoleId))
        {
            var policy = await DbContext.Policies.Where(new EntityByKeySpecification<Policy, Guid>(PolicyConstantId.FullAccessPolicyId))
                .SingleAsync(CancellationToken.None);
            await dbContext.Roles.AddAsync(new Role
            {
                Id = RoleConstantId.SuperAdminRoleId,
                Name = "Полные права",
                Policies = new[]
                {
                    policy
                }
            });
        }

        if (!await dbContext.Users.AnyAsync())
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@test.com",
                RoleId = RoleConstantId.SuperAdminRoleId,
                FirstName = "Андрей",
                LastName = "Топчу",
                MiddleName = "Дмитриевич",
                EmailConfirmed = _systemClock.UtcNow.UtcDateTime,
            };
            admin.Password = _passwordHashGenerator.MakeHashWithSalt(admin.Id, "test_password");
            await dbContext.Users.AddAsync(admin);
        }
    }

    private async Task DevelopmentSeed(IdentityDbContext dbContext)
    {
        if (!await dbContext.Policies.AnyAsync(x => x.Id == PolicyConstantId.FullAccessPolicyId))
        {
            await dbContext.Policies.AddAsync(new Policy
            {
                Id = PolicyConstantId.FullAccessPolicyId,
                Name = "fullAccess",
                Clients = new ClientPolicy[]
                {
                    new() {ClientId = ClientConstantId.IdentityClientId, PolicyId = PolicyConstantId.FullAccessPolicyId, PolicyName = "fullAccess"}
                }
            });
            await dbContext.SaveChangesAsync(CancellationToken.None);
        }

        if (!await dbContext.Roles.AnyAsync(x => x.Id == RoleConstantId.SuperAdminRoleId))
        {
            var policy = await DbContext.Policies.Where(new EntityByKeySpecification<Policy, Guid>(PolicyConstantId.FullAccessPolicyId))
                .SingleAsync(CancellationToken.None);
            await dbContext.Roles.AddAsync(new Role
            {
                Id = RoleConstantId.SuperAdminRoleId,
                Name = "Полные права",
                Policies = new[]
                {
                    policy
                }
            });
        }

        if (!await dbContext.Users.AnyAsync())
        {
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@test.com",
                RoleId = RoleConstantId.SuperAdminRoleId,
                FirstName = "Андрей",
                LastName = "Топчу",
                MiddleName = "Дмитриевич",
                EmailConfirmed = _systemClock.UtcNow.UtcDateTime,
            };
            admin.Password = _passwordHashGenerator.MakeHashWithSalt(admin.Id, "test_password");
            await dbContext.Users.AddAsync(admin);
        }
    }
}