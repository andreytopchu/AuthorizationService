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
                        JwtClaimTypes.Name
                    }
                },
                new("policy", new[] { "policy" })
            };
    }
}