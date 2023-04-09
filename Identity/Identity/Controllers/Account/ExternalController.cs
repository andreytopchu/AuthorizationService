using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local

namespace Identity.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<ExternalController> _logger;
        private readonly UserStore.UserStoreClient _userStoreClient;
        private readonly ISocialAuthTokenGeneratorService _socialAuthTokenGeneratorService;
        private const string ClientName = "social.client";

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string scheme, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            // start challenge and roundtrip the return URL and scheme
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
            };

            return Challenge(props, scheme);
        }

        public ExternalController(IIdentityServerInteractionService interaction, ISocialAuthTokenGeneratorService socialAuthTokenGeneratorService,
            IEnumerable<UserStore.UserStoreClient> clients, ILogger<ExternalController> logger)
        {
            _socialAuthTokenGeneratorService = socialAuthTokenGeneratorService ?? throw new ArgumentNullException(nameof(socialAuthTokenGeneratorService));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userStoreClient = clients.FirstOrDefault(x => x.Name.StartsWith(ClientName))
                               ?? throw new InvalidOperationException($"User client storage with name {ClientName}, not found");
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<TokenResponse> Callback()
        {
            // read external identity from the temporary cookie
            var result =
                await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@Claims}", (object)externalClaims ?? "null");
            }

            // lookup our user and external provider info
            var (user, provider, providerUserId, claims) = FindUserFromExternalProvider(result);
            if (user == null)
            {
                //добавить нового пользователя, если будет необходимо

                AutoProvisionUser(provider, providerUserId, claims);
                // this might be where you might initiate a custom workflow for user registration
                // in this sample we don't show how that would be done, as our sample implementation
                // simply auto-provisions new external user
            }

            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var tokenResponse = await _socialAuthTokenGeneratorService.GenerateTokenForExternalProviderUser(user?.Sub, ClientName);
            return tokenResponse;
        }

        private (User user, string provider, string providerUserId, IEnumerable<Claim> claims) FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;
            if (externalUser == null)
                throw new InvalidOperationException("Principal is null");

            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                              throw new Exception("Unknown userid");

            var providerName = result.Properties?.Items["scheme"] ?? throw new InvalidOperationException("Provider name is null");
            var externalUserId = userIdClaim.Value;

            // find external user
            _logger.LogDebug("Find user [{@UserId}] with provider [{@ProviderName}]", externalUserId, providerName);
            var user = _userStoreClient.FindByExternalProvider(new UserFilterByProvider { Provider = providerName, UserId = externalUserId });

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.Where(x => x != userIdClaim).ToList();

            return (user, providerName, externalUserId, claims);
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private User AutoProvisionUser(string provider, string providerUserId, IEnumerable<Claim> claims)
        {
            throw new NotImplementedException();
            // return _userStoreClient.AutoProvisionUser(provider, providerUserId, claims.ToList());
        }

        private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal?.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var idToken = externalResult.Properties.GetTokenValue("id_token");
            if (idToken != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
            }
        }
    }
}