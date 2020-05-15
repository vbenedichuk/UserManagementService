using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace UserManagementService.Configuration
{
    public static class IdentityServerConfigurationHelper
    {
        public static IEnumerable<ApiResource> GetApis()
        {
            return new []
            {
                new ApiResource("UserManagement", new []{"role" })
            };
        }

        public static IEnumerable<Scope> GetScopes()
        {
            return new []
            {
                new Scope("patientManagement", "Patient Management", new []{ "patient/*", JwtClaimTypes.Role }),
                new Scope("UserManagement", "User Management", new []{ JwtClaimTypes.Role }),
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("role", "User roles", new[] { JwtClaimTypes.Role })
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                new Client
                {
                    ClientId = "UserManagement",
                    ClientName = "User Management App",
                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 330,
                    IdentityTokenLifetime = 30,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = false,
                    RequireClientSecret = false,
                    RequirePkce = true,
                    RequireConsent = false,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "UserManagement",
                        "role"
                    },

                    // where to redirect to after login
                    RedirectUris = { 
                        "http://localhost:4200/callback", 
                        "http://localhost:4200/silent-renew.html",
                        "https://auth.localservice/",
                        "https://auth.localservice/silent-renew.html" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { 
                        "http://localhost:4200/logout",
                        "https://auth.localservice/auth/logout"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200",
                        "http://localhost:8080",
                        "https://auth.localservice"
                    },
                }
            };
        }
    }
}
