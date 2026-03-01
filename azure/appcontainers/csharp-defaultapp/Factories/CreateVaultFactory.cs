using System.Linq;
using Pulumi;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;

namespace defaultapp.Factories;

public static class CreateVaultFactory
{
    public static Vault Create(CreateVaultArgs args)
    {
        var networkAclRuleSet = args.EnableNetworkAcl
            ? new NetworkRuleSetArgs
            {
                Bypass = args.NetworkAclRuleSet.Bypass,
                DefaultAction = args.NetworkAclRuleSet.DefaultAction,
                IpRules = args.NetworkAclRuleSet.IpRules?.Select(ipRule => new IPRuleArgs
                {
                    Value = ipRule
                }).ToList()!,
                VirtualNetworkRules = args.NetworkAclRuleSet.VirtualNetworkRules?.Select(vnetRule => new VirtualNetworkRuleArgs
                {
                    Id = vnetRule
                }).ToList() !
            }
            : null;
        return new Vault($"OneBank_Vault_{args.Name}", new VaultArgs
        {
            VaultName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location =  args.Location,
            Properties = new VaultPropertiesArgs
            {
                EnabledForDeployment = args.EnabledForTemplateDeployment,
                EnableSoftDelete = args.EnableSoftDelete,
                EnableRbacAuthorization =  args.EnableRbacAuthorization,
                SoftDeleteRetentionInDays =  args.SoftDeleteRetentionInDays,
                PublicNetworkAccess =  args.PublicNetworkAccess,
                Sku = new SkuArgs
                {
                    Name = args.SkuName,
                    Family = args.SkuFamily
                },
                NetworkAcls = networkAclRuleSet!
            },
            Tags =  args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}