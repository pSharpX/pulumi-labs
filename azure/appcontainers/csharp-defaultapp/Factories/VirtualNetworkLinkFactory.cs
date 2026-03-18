using Pulumi;
using Pulumi.AzureNative.PrivateDns;
using Pulumi.AzureNative.PrivateDns.Inputs;

namespace defaultapp.Factories;

public static class VirtualNetworkLinkFactory
{
    public static VirtualNetworkLink Create(CreateVirtualNetworkLinkArgs args)
    {
        return new VirtualNetworkLink($"OneBank_VirtualNetworkLink_{args.Alias}", new VirtualNetworkLinkArgs
        {
            VirtualNetworkLinkName =  args.Name,
            PrivateZoneName = args.PrivateZoneName,
            VirtualNetwork = new SubResourceArgs
            {
              Id  = args.VirtualNetworkId
            },
            ResourceGroupName = args.ResourceGroupName,
            Location =  args.Location,
            RegistrationEnabled =  args.RegistrationEnabled,
            Tags =  args.Tags!,

        }, new CustomResourceOptions { Parent = args.Parent });
    }
}