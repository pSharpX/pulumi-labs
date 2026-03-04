using Pulumi;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.ContainerRegistry.Inputs;

namespace defaultapp.Factories;

public static class ContainerRegistryFactory
{
    public static Registry Create(CreateContainerRegistryArgs args)
    {
        EncryptionPropertyArgs? encryptionSettings = null;
        if (args.EncryptionEnabled)
        {
            encryptionSettings = new EncryptionPropertyArgs
            {
                KeyVaultProperties = new KeyVaultPropertiesArgs
                {
                    Identity =  args.VaultIdentity,
                    KeyIdentifier =  args.VaultKeyIdentifier,
                },
                Status = EncryptionStatus.Enabled,
            };
        }
        return new Registry("OneBank_ContainerRegistry", new RegistryArgs()
        {
            RegistryName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Sku = new SkuArgs
            {
                Name = args.SkuName,
            },
            AdminUserEnabled =  args.AdminUserEnabled,
            PublicNetworkAccess =  args.PublicNetworkAccess,
            NetworkRuleBypassOptions =  args.NetworkRuleBypassOptions,
            AnonymousPullEnabled =   args.AnonymousPullEnabled,
            Encryption = encryptionSettings!,
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}