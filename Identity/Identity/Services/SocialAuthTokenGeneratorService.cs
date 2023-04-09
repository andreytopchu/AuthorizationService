using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Stores;
using IdentityServer4.Validation;

namespace Identity.Services
{
    public class SocialAuthTokenGeneratorService : ISocialAuthTokenGeneratorService
    {
        private readonly IScopeParser _scopeParser;
        private readonly IResourceStore _resourceStore;
        private readonly ITokenResponseGenerator _tokenResponseGenerator;
        private readonly IClientStore _clientStore;

        public SocialAuthTokenGeneratorService(IScopeParser scopeParser, IResourceStore resourceStore,
            ITokenResponseGenerator tokenResponseGenerator, IClientStore clientStore)
        {
            _tokenResponseGenerator = tokenResponseGenerator ?? throw new ArgumentNullException(nameof(tokenResponseGenerator));
            _scopeParser = scopeParser ?? throw new ArgumentNullException(nameof(scopeParser));
            _resourceStore = resourceStore ?? throw new ArgumentNullException(nameof(resourceStore));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        }

        public async Task<TokenResponse> GenerateTokenForExternalProviderUser(string sub, string clientName)
        {
            if (sub == null) throw new ArgumentNullException(nameof(sub));
            if (clientName == null) throw new ArgumentNullException(nameof(clientName));

            var client = await _clientStore.FindEnabledClientByIdAsync(clientName);
            var grantValidationResult = new GrantValidationResult(sub, OidcConstants.AuthenticationMethods.Password);

            var parsedScopesResult = _scopeParser.ParseScopeValues(client.AllowedScopes);
            var validatedResources = await _resourceStore.CreateResourceValidationResult(parsedScopesResult);
            validatedResources.Resources.OfflineAccess = client.AllowOfflineAccess;

            var tokenRequestValidationResult = new TokenRequestValidationResult(
                    new ValidatedTokenRequest
                    {
                        ClientId = client.ClientId,
                        AccessTokenLifetime = client.AccessTokenLifetime,
                        AccessTokenType = client.AccessTokenType,
                        ClientClaims = client.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)).ToList(),
                        GrantType = OidcConstants.AuthenticationMethods.Password,
                        Client = client,
                        Subject = grantValidationResult.Subject,
                        ValidatedResources = validatedResources
                    })
                ;

            return await _tokenResponseGenerator.ProcessAsync(tokenRequestValidationResult);
        }
    }
}