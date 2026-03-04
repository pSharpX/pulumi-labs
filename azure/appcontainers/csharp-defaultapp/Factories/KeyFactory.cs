using System;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;

namespace defaultapp.Factories;

public static class KeyFactory
{
    public static Key Create(CreateKeyArgs args)
    {
        return new Key("OneBank_Vault_Key", new KeyArgs
        {
            KeyName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            VaultName = args.VaultName,
            Properties = new KeyPropertiesArgs
            {
                Attributes = new KeyAttributesArgs
                {
                    Enabled = args.Enabled,
                    Expires = args.Expires,
                    Exportable = args.Exportable,
                    NotBefore = args.NotBefore,
                },
                CurveName = args.CurveName!,
                KeyOps = { "encrypt", "decrypt", "sign", "verify", "wrapKey", "unwrapKey", "import", "release" },
                KeySize = args.KeySize,
                Kty = args.KeyType!,
            },
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}