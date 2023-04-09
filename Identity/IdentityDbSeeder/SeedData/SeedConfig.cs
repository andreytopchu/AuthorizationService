// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using IdentityModel;
using IdentityServer4.Models;

namespace IdentityDbSeeder.SeedData
{
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
                        JwtClaimTypes.Name,
                        JwtClaimTypes.ClientId,
                    }
                },
                new("policy", new[] { "policy" })
            };

        public static IEnumerable<ApiResource> ApiResources => new[]
        {
            new ApiResource("s.profile", "Profile service API")
            {
                Scopes = new List<string> { "mobile-api", "admin-api", "webclient-api", "device-api" },
                UserClaims = new List<string> { "policy", "session_id", "name", "hash"},
                ApiSecrets = SecretConfig.ProfileApiResourceSecrets
            },
            new ApiResource("s.mobile", "Mobile service API")
            {
                Scopes = new List<string> { "mobile-api", "admin-api", "webclient-api" },
                UserClaims = new List<string> { "policy" },
                ApiSecrets = SecretConfig.MobileApiResourceSecrets
            },
            new ApiResource("s.admin", "Admin API")
            {
                Scopes = new List<string> { "admin-api" },
                UserClaims = new List<string> { "policy" },
                ApiSecrets = SecretConfig.AdminApiResourceSecrets
            },
        };

        public static IEnumerable<ApiScope> ApiScopes => new[]
        {
            new ApiScope("mobile-api"),
            new ApiScope("webclient-api"),
            new ApiScope("admin-api"),
            new ApiScope("device-api"),
        };

        public static IEnumerable<Client> Clients => new[]
        {
            new Client
            {
                ClientName = "Mobile device auth",
                ClientId = "mobile.device",
                ClientSecrets = SecretConfig.MobileDeviceSecrets,
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "device-api" },
                AccessTokenLifetime = 10 * 60, // 10 min
            },

            new Client
            {
                ClientName = "Mobile API",
                ClientSecrets = SecretConfig.MobileApiClientSecrets,
                ClientId = "mobile.client",
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "openid", "profile", "mobile-api" },

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8 час
                AbsoluteRefreshTokenLifetime = 365 * 24 * 3600, // 365 дней
                SlidingRefreshTokenLifetime = 30 * 24 * 3600, // 30 дней
            },

            new Client
            {
                ClientName = "From Social API",
                ClientSecrets = SecretConfig.MobileApiClientSecrets,
                ClientId = "social.client",
                AccessTokenType = AccessTokenType.Jwt,

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "openid", "profile", "mobile-api", "social-api", "offline_access" },
                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8 час
                AbsoluteRefreshTokenLifetime = 365 * 24 * 3600, // 365 дней
                SlidingRefreshTokenLifetime = 30 * 24 * 3600, // 30 дней
            },

            new Client
            {
                ClientName = "WebCabinet API",
                ClientSecrets = SecretConfig.WebApiClientSecrets,
                ClientId = "web.client",
                AccessTokenType = AccessTokenType.Jwt,
                AllowedCorsOrigins = new List<string>
                {
                    "https://web.template.online",
                    "https://test.k3s.spartak.com",
                    "https://dev.k3s.spartak.com",
                    "https://stage.k3s.spartak.com",
                    "https://shop.k3s.spartak.com",
                    "https://spartak.com",
                    "http://spartak.local:4100",
                    "https://new.spartak.com",
                    "https://newstore.spartak.com"
                },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "openid", "profile", "webclient-api" },

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 3600, // 1 hours
                AbsoluteRefreshTokenLifetime = 30 * 24 * 3600, // 30 days
                SlidingRefreshTokenLifetime = 7 * 24 * 3600 // 7 days
            },

            new Client
            {
                ClientName = "Admin API",
                ClientSecrets = SecretConfig.AdminApiClientSecrets,
                ClientId = "admin.client",
                AccessTokenType = AccessTokenType.Reference,
                AllowedCorsOrigins = new List<string>
                {
                    "https://admin-dev.k3s.spartak.com",
                    "https://admin.k3s.spartak.com",
                    "http://localhost.admin-dev.template.online:3000",
                    "https://admin-stage.k3s.spartak.com",
                    "https://admin-test.k3s.spartak.com",
                    "https://adminnew.spartak.com"
                },

                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                AllowedScopes = { "openid", "profile", "admin-api", "policy" },

                AllowOfflineAccess = true,
                UpdateAccessTokenClaimsOnRefresh = true,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Sliding,
                AccessTokenLifetime = 8 * 3600, // 8h
                AbsoluteRefreshTokenLifetime = 30 * 24 * 3600, // 30 days
                SlidingRefreshTokenLifetime = 7 * 24 * 3600 // 7 days
            }
        };
    }
}