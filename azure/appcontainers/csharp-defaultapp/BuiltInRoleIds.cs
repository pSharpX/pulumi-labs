using System.Collections.Generic;

namespace defaultapp;

public static class BuiltInRoleIds
{
    private static readonly IReadOnlyDictionary<BuiltInRole, string> Map =
        new Dictionary<BuiltInRole, string>
        {
            [BuiltInRole.Reader] = "acdd72a7-3385-48ef-bd42-f606fba81ae7",
            [BuiltInRole.Contributor] = "b24988ac-6180-42a0-ab88-20f7382dd24c",
            [BuiltInRole.StorageBlobDataReader] = "2a2b9908-6ea1-4ae2-8e65-a410df84e7d1",
            [BuiltInRole.KeyVaultSecretsUser] = "4633458b-17de-408a-b874-0445c86b69e6",
            [BuiltInRole.AcrPull] = "7f951dda-4ed3-4680-a7ca-43fe172d538d",
            [BuiltInRole.AcrPush] = "8311e382-0749-4cb8-b61a-304f252e45ec",
            [BuiltInRole.AppConfigurationDataReader] = "516239f1-63e1-4d78-a4de-a74fb236a071",
        };

    public static string Get(BuiltInRole role) => Map[role];
}