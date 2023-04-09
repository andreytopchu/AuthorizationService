using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Grpc.Core;
using Identity.Options;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Identity.Services
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ISystemClock _clock;
        private readonly IEnumerable<UserStore.UserStoreClient> _clients;
        private readonly IOptions<IdentityOptions> _identityOptions;

        public ResourceOwnerPasswordValidator(ISystemClock clock, IEnumerable<UserStore.UserStoreClient> clients,
            IOptions<IdentityOptions> identityOptions)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _identityOptions = identityOptions ?? throw new ArgumentNullException(nameof(identityOptions));
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
                return;

            if (string.IsNullOrWhiteSpace(context.Request.ClientId))
                throw new ArgumentNullException(nameof(context.Request.ClientId));

            var credential = new Credential
            {
                Username = context.UserName,
                Password = context.Password,
                ClientId = context.Request.ClientId
            };

            try
            {
                var result = await GetClient(context.Request.ClientId).ValidateCredentialsAsync(credential);
                if (result.Success)
                {
                    var grantValidationResult = new GrantValidationResult(
                        result.Sub,
                        OidcConstants.AuthenticationMethods.Password,
                        _clock.UtcNow.UtcDateTime,
                        identityProvider: _identityOptions.Value.ProviderName
                    );
                    grantValidationResult.Subject.AddIdentity(new ClaimsIdentity(new List<Claim> {new("session_id", result.Session), new("hash", result.Hash) }));
                    context.Result = grantValidationResult;
                }
                else
                {
                    context.Result.IsError = true;
                    context.Result.ErrorDescription = result.Message;

                    if (result.ErrorCode == AuthErrorCode.BruteForce)
                    {
                        context.Result.CustomResponse = new Dictionary<string, object>
                        {
                            {nameof(result.NoUntil), result.NoUntil.ToString()}
                        };
                    }
                }
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.NotFound)
            {
                // pass
            }
        }

        private UserStore.UserStoreClient GetClient(string clientId)
        {
            return _clients.First(x => x.Name.StartsWith(clientId));
        }
    }
}