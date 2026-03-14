using System.Linq;
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
        InputList<SubnetArgs> subnets = [];
        if (args.Subnets is not null && args.Subnets.Count > 0)
        {
            subnets = args.Subnets.Select(subnet => new SubnetArgs
            {
                AddressPrefix =  subnet.SubnetAddressPrefix,
                AddressPrefixes = subnet.SubnetAddressPrefixes!,
                
                Name = subnet.Name,
                PrivateEndpointNetworkPolicies = "Enabled",
                PrivateLinkServiceNetworkPolicies = "Enabled",
                Delegations = subnet.Delegations?.Select(delegation => new DelegationArgs
                {
                    Name = delegation.Item1,
                    ServiceName = delegation.Item2,
                }).ToList()!,
            }).ToList();
        }
        return new VirtualNetwork("OneBank_VirtualNetwork", new VirtualNetworkArgs
        {
            VirtualNetworkName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AddressSpace = new AddressSpaceArgs
            {
                AddressPrefixes =  args.AddressPrefixes,
            },
            Subnets = subnets,
            Tags = args.Tags!
        }, new CustomResourceOptions
        {
            Parent = args.Parent
        });
    }
}