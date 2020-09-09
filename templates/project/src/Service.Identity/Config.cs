// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the demo root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace Service.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("webapp"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "webapp",
                    ClientName = "Ng Web Client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    //ClientSecrets = { new Secret("secret".Sha256()) },

                    RedirectUris = { "http://localhost:4200/index.html" },
                    FrontChannelLogoutUri = "http://localhost:4200/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:4200/signout-callback-oidc" },
                    AllowedCorsOrigins =     { "http://localhost:4200","https://localhost:4200" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "webapp", "offline_access" }
                },
            };
    }
}
