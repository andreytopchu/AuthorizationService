using Dex.Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.Extensions.Logging;

namespace Identity.Services
{
    public class InvalidateUserTokenService : IInvalidateUserTokenService
    {
        private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ILogger<InvalidateUserTokenService> _logger;

        public InvalidateUserTokenService(PersistedGrantDbContext persistedGrantDbContext, ILogger<InvalidateUserTokenService> logger)
        {
            _persistedGrantDbContext = persistedGrantDbContext ?? throw new ArgumentNullException(nameof(persistedGrantDbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvalidateToken(Guid[] userIds, CancellationToken cancellationToken)
        {
            if (userIds.IsNullOrEmpty())
            {
                _logger.LogWarning("Nothing to invalidate, users list are empty");
                return;
            }

            var needInvalidateTokens = _persistedGrantDbContext.PersistedGrants
                .Where(x => userIds.Select(y => y.ToString()).Contains(x.SubjectId));

            _persistedGrantDbContext.RemoveRange(needInvalidateTokens);
            await _persistedGrantDbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tokens are successeful invalidated. UserIds: {UserIds}", userIds);
        }
    }
}