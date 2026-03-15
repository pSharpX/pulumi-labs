using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Network;
using ApplicationSecurityGroupArgs = Pulumi.AzureNative.Network.Inputs.ApplicationSecurityGroupArgs;

namespace defaultapp.Factories;

public static class SecurityRuleFactory
{
    public static SecurityRule Create(CreateSecurityRuleArgs args)
    {
        return new SecurityRule($"OneBank_SecurityRule_{args.Alias}", new SecurityRuleArgs
        {
            Name = args.Name,
            Description = args.Description!,
            ResourceGroupName = args.ResourceGroupName,
            NetworkSecurityGroupName = args.NetworkSecurityGroupName!,
            Access = args.Access,
            Direction = args.Direction,
            Protocol = args.Protocol,
            DestinationAddressPrefix = args.DestinationAddressPrefix!,
            DestinationAddressPrefixes = args.DestinationAddressPrefixes!,
            DestinationApplicationSecurityGroups = args.DestinationApplicationSecurityGroups?
                .Select(asp => new ApplicationSecurityGroupArgs
                {
                    Id = asp,
                    Location = args.Location
                }).ToList()!,
            DestinationPortRange = args.DestinationPortRange!,
            DestinationPortRanges = args.DestinationPortRanges!,
            SourceAddressPrefix = args.SourceAddressPrefix!,
            SourceAddressPrefixes = args.SourceAddressPrefixes!,
            SourceApplicationSecurityGroups = args.SourceApplicationSecurityGroups?
                .Select(asp => new ApplicationSecurityGroupArgs
                {
                    Id = asp,
                    Location = args.Location
                }).ToList()!,
            SourcePortRange = args.SourcePortRange!,
            SourcePortRanges = args.SourcePortRanges!,
            Priority = args.Priority,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}