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
                new ApiResource("UserManagement")
            };
        }

        public static IEnumerable<Scope> GetScopes()
        {
            return new []
            {
                new Scope("patientManagement", "Patient Management", new []{ "patient/*", JwtClaimTypes.Role }),
                new Scope("UserManagement", "User Management", new []{ "user/*", JwtClaimTypes.Role }),
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

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "UserManagement",
                        "role"
                    },

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:4200/callback" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:4200/logout" },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200",
                        "http://localhost:8080"
                    },
                },
                new Client
                {
                    ClientId = "PatientManagement",
                    ClientName = "Patient Management App",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.Code,
                    // Specifies whether this client can request refresh tokens
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RequirePkce = true,

                    // no consent page
                    RequireConsent = false,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    },

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:8080/callback.html" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:8080/logout.html" }
                }
            };
        }
    }
}
