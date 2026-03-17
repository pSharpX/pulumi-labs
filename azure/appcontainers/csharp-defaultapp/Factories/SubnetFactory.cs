using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using NetworkSecurityGroupArgs = Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs;
using SubnetArgs = Pulumi.AzureNative.Network.SubnetArgs;

namespace defaultapp.Factories;

public static class SubnetFactory
{
    public static Subnet Create(CreateSubnetArgs args)
    {
        InputList<DelegationArgs> delegations = [];
        if (args.Delegations is not null && args.Delegations.Count > 0)
        {
            delegations = args.Delegations.Select(delegation => new DelegationArgs
            {
                Name = delegation.Item1,
                ServiceName = delegation.Item2
            }).ToList();
        }

        NetworkSecurityGroupArgs? networkSecurityGroup = null;
        if (args.NetworkSecurityGroupId is not null)
        {
            networkSecurityGroup = new NetworkSecurityGroupArgs
            {
                Id = args.NetworkSecurityGroupId,
            };
        }
        
        return new Subnet($"OneBank_SubNet_{args.Alias}", new SubnetArgs
        {
            Name =  args.Name,
            SubnetName = args.SubnetName,
            VirtualNetworkName = args.VirtualNetworkName,
            AddressPrefix = args.SubnetAddressPrefix,
            AddressPrefixes = args.SubnetAddressPrefixes!,
            ResourceGroupName = args.ResourceGroupName,
            NetworkSecurityGroup =  networkSecurityGroup!,
            Delegations = delegations 
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}