using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using Npgsql;

namespace Identity.Dal
{
    /// <summary>
    /// Design factory
    /// </summary>
    internal class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
    {
        public IdentityDbContext CreateDbContext(string[] args)
        {
            var sc = CreateServiceCollection();

            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            builder.UseNpgsql();
            builder.UseApplicationServiceProvider(sc.BuildServiceProvider());

            return CreateInstance(builder.Options);
        }

        protected virtual ServiceCollection CreateServiceCollection()
        {
            var sc = new ServiceCollection();
            sc.AddSingleton<ISystemClock, SystemClock>();
#pragma warning disable CS0618
            sc.AddSingleton(NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator);
#pragma warning restore CS0618
            return sc;
        }

        private static IdentityDbContext CreateInstance(DbContextOptions<IdentityDbContext> options)
        {
            return new IdentityDbContext(options);
        }
    }
}