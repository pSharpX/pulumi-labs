using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using PublicIPAddressArgs = Pulumi.AzureNative.Network.PublicIPAddressArgs;

namespace defaultapp.Factories;

public static class PublicIpAddressFactory
{
    public static PublicIPAddress Create(CreatePublicIpAddressArgs args)
    {
        return new PublicIPAddress($"OneBank_PublicIPAddress_{args.DnsNameLabel}", new PublicIPAddressArgs
        {
            ResourceGroupName =  args.ResourceGroupName,
            Location =  args.Location,
            PublicIpAddressName = args.Name,
            PublicIPAddressVersion =  args.IpVersion,
            PublicIPAllocationMethod = args.IpAllocationMethod,
            Sku = new PublicIPAddressSkuArgs
            {
                Name = args.SkuName,
                Tier = args.SkuTier
            },
            IdleTimeoutInMinutes = args.IdleTimeoutInMinutes,
            DnsSettings = new PublicIPAddressDnsSettingsArgs
            {
                DomainNameLabel = args.DnsNameLabel,
            },
            DeleteOption = args.DeleteOption,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}