using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;

namespace defaultapp.Factories;

public static class CreateSecretFactory
{
    public static Secret Create(CreateSecretArgs args)
    {
        return new Secret($"OneBank_Secret_{args.Name}", new SecretArgs
        {
            SecretName =  args.Name,
            VaultName =  args.VaultName,
            ResourceGroupName =  args.ResourceGroupName,
            Properties = new SecretPropertiesArgs
            {
              Value  =  args.Value,
              ContentType =  args.ContentType,
              Attributes = new SecretAttributesArgs
              {
                  Enabled =  args.Enabled,
                  Expires =  args.Expires,
                  NotBefore = args.NotBefore
              },
            },
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}