using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Dal
{
    public class SecurityTokenDbContext : BaseDbContext<SecurityTokenDbContext>, IDataProtectionKeyContext
    {
        public SecurityTokenDbContext(DbContextOptions<SecurityTokenDbContext> options) : base(options)
        {
        }

        public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    }
}