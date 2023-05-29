using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Application.Abstractions.Models.Authorization;
using Identity.Application.Abstractions.Services;
using Identity.Options;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Identity.Services;

// ReSharper disable once ClassNeverInstantiated.Global
public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly ISystemClock _clock;
    private readonly IUserStoreService _userStoreService;
    private readonly IOptions<IdentityOptions> _identityOptions;

    public ResourceOwnerPasswordValidator(ISystemClock clock, IUserStoreService userStoreService,
        IOptions<IdentityOptions> identityOptions)
    {
        _clock = clock;
        _userStoreService = userStoreService;
        _identityOptions = identityOptions;
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


        var result = await _userStoreService.ValidateCredentials(credential);
        if (result.Success)
        {
            context.Result = new GrantValidationResult(
                result.Sub,
                OidcConstants.AuthenticationMethods.Password,
                _clock.UtcNow.UtcDateTime,
                identityProvider: _identityOptions.Value.ProviderName
            );
        }
        else
        {
            context.Result.IsError = true;
            context.Result.ErrorDescription = result.Message;

            if (result.ErrorCode == AuthErrorCode.BruteForce)
            {
                context.Result.CustomResponse = new Dictionary<string, object>
                {
                    { nameof(result.NoUntil), result.NoUntil.ToString() }
                };
            }
        }
    }
}