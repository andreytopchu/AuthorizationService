using System.Threading.Tasks;
using IdentityServer4.ResponseHandling;

namespace Identity.Services
{
    public interface ISocialAuthTokenGeneratorService
    {
        Task<TokenResponse> GenerateTokenForExternalProviderUser(string sub, string clientName);
    }
}