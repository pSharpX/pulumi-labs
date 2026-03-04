using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.AppConfiguration.Inputs;

namespace defaultapp.Factories;

public static class ConfigurationStoreFactory
{
    public static ConfigurationStore Create(CreateConfigurationStoreArgs args)
    {
        EncryptionPropertiesArgs? encryptionPropertiesArgs = null;
        if (args.EncryptionEnabled)
        {
            encryptionPropertiesArgs = new EncryptionPropertiesArgs
            {
                KeyVaultProperties = new KeyVaultPropertiesArgs
                {
                    IdentityClientId = args.VaultIdentity,
                    KeyIdentifier = args.VaultKeyIdentifier
                }
            };
        }
        return new ConfigurationStore($"OneBank_ConfigStore", new ConfigurationStoreArgs
        {
            ConfigStoreName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            DisableLocalAuth = args.DisableLocalAuth,
            EnablePurgeProtection = args.EnablePurgeProtection,
            PublicNetworkAccess = args.PublicNetworkAccess,
            Sku = new SkuArgs
            {
                Name = args.SkuName,
            },
            SoftDeleteRetentionInDays =  args.SoftDeleteRetentionInDays,
            Encryption =encryptionPropertiesArgs!, 
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}