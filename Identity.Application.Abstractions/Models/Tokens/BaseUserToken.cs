using Dex.SecurityTokenProvider.Models;

namespace Identity.Application.Abstractions.Models.Tokens
{
    public class BaseUserToken : BaseToken
    {
        // ReSharper disable once EmptyConstructor
        // ReSharper disable once MemberCanBeProtected.Global
        public BaseUserToken()
        {
        }

        public Guid UserId { get; set; }
    }
}