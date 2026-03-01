using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using SubnetArgs = Pulumi.AzureNative.Network.Inputs.SubnetArgs;
using VirtualNetworkArgs = Pulumi.AzureNative.Network.VirtualNetworkArgs;

namespace defaultapp.Factories;

public static class VirtualNetworkFactory
{
    public static VirtualNetwork Create(CreateVirtualNetworkArgs args)
    {
        return new VirtualNetwork("OneBank_VirtualNetwork", new VirtualNetworkArgs
        {
            VirtualNetworkName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AddressSpace = new AddressSpaceArgs
            {
                AddressPrefixes =  args.AddressPrefixes,
            },
            Subnets = new[]
            {
                new SubnetArgs
                {
                    AddressPrefixes = args.SubnetAddressPrefixes,
                    Name = $"{args.Name}-Subnet",
                    PrivateEndpointNetworkPolicies = "Enabled",
                    PrivateLinkServiceNetworkPolicies = "Enabled"
                },
            },
            Tags = args.Tags!
        }, new CustomResourceOptions
        {
            Parent = args.Parent
        });
    }
}