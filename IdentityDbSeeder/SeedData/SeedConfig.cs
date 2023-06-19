// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Identity.Domain.Constants;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityDbSeeder.SeedData;

public static class SeedConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile
            {
                UserClaims = new List<string>
                {
                    JwtClaimTypes.Name
                }
            },
            new("policy", new[] {"policy"})
        };

    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("s.identity", "Identity API")
        {
            Scopes = new List<string> {"identity-api"},
            UserClaims = new List<string> {"policy"},
            ApiSecrets = new List<Secret> {new("84C2198A-20DF-4647-8308-124BB6EF2093".ToSha256())}
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("identity-api"),
    };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientName = "Identity API",
            ClientSecrets = new List<Secret>
            {
                new("F3A22D2A-1C16-4B6C-809F-8A9D01A86EE1".ToSha256())
            },
            ClientId = ClientConstantId.IdentityClientId,
            AccessTokenType = AccessTokenType.Reference,
            AllowedCorsOrigins = new List<string>(),

            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            AllowedScopes = {"openid", "identity-api"},

            AllowOfflineAccess = true,
            UpdateAccessTokenClaimsOnRefresh = true,
            RefreshTokenUsage = TokenUsage.ReUse,
            RefreshTokenExpiration = TokenExpiration.Sliding,
            AccessTokenLifetime = 8 * 3600, // 8h
            AbsoluteRefreshTokenLifetime = 30 * 24 * 3600, // 30 days
            SlidingRefreshTokenLifetime = 7 * 24 * 3600 // 7 days
        },
    };
}