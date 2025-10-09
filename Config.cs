//using Microsoft.AspNetCore.DataProtection;
//using MySqlX.XDevAPI;

//namespace ShairiStore;

//public static class Config
//{
//    public static IEnumerable<IdentityResource> IdentityResources =>
//        new List<IdentityResource>
//        {
//                new IdentityResources.OpenId(),
//                new IdentityResources.Profile(),
//                new IdentityResource("roles", new[] { "role" }) // 👈 adds role claim support
//        };

//    public static IEnumerable<ApiScope> ApiScopes =>
//        new List<ApiScope>
//        {
//                new ApiScope("shairistore_api", "ShairiStore API")
//        };

//    public static IEnumerable<ApiResource> ApiResources =>
//        new List<ApiResource>
//        {
//                new ApiResource("shairistore_api", "ShairiStore API")
//                {
//                    Scopes = { "shairistore_api" },
//                    UserClaims = { "role" }
//                }
//        };

//    public static IEnumerable<Client> Clients =>
//        new List<Client>
//        {
//                new Client
//                {
//                    ClientId = "shairistore_client",
//                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
//                    ClientSecrets = { new Secret("super_secret_key".Sha256()) },
//                    AllowedScopes =
//                    {
//                        "openid", "profile", "roles", "shairistore_api"
//                    },
//                    AllowOfflineAccess = true
//                }
//        };
//}
