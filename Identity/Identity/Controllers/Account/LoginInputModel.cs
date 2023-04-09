// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Shared.ValidationSpartak.Attributes;

namespace Identity.Controllers.Account
{
    public class LoginInputModel
    {
        [CustomRequired]
        public string Username { get; set; }

        [CustomRequired]
        public string Password { get; set; }

        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}