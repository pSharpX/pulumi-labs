using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Network;
using SecurityRuleArgs = Pulumi.AzureNative.Network.Inputs.SecurityRuleArgs;
using ApplicationSecurityGroupArgs = Pulumi.AzureNative.Network.Inputs.ApplicationSecurityGroupArgs;

namespace defaultapp.Factories;

public static class NetworkSecurityGroupFactory
{
    public static NetworkSecurityGroup Create(CreateNetworkSecurityGroupArgs args)
    {
        return new NetworkSecurityGroup($"OneBank_NSG_{args.Alias}", new NetworkSecurityGroupArgs
        {
            NetworkSecurityGroupName =  args.Name,
            SecurityRules = args.SecurityRules.Select(rule => new SecurityRuleArgs()
            {
                Name = args.Name,
                Description = rule.Description!,
                Access = rule.Access,
                Direction = rule.Direction,
                Protocol = rule.Protocol,
                DestinationAddressPrefix = rule.DestinationAddressPrefix!,
                DestinationAddressPrefixes = rule.DestinationAddressPrefixes!,
                DestinationApplicationSecurityGroups = rule.DestinationApplicationSecurityGroups?
                    .Select(asp => new ApplicationSecurityGroupArgs
                    {
                        Id = asp,
                        Location = args.Location
                    }).ToList()!,
                DestinationPortRange = rule.DestinationPortRange!,
                DestinationPortRanges = rule.DestinationPortRanges!,
                SourceAddressPrefix = rule.SourceAddressPrefix!,
                SourceAddressPrefixes = rule.SourceAddressPrefixes!,
                SourceApplicationSecurityGroups = rule.SourceApplicationSecurityGroups?
                    .Select(asp => new ApplicationSecurityGroupArgs
                    {
                        Id = asp,
                        Location = args.Location
                    }).ToList()!,
                SourcePortRange = rule.SourcePortRange!,
                SourcePortRanges = rule.SourcePortRanges!,
                Priority = rule.Priority,
            }).ToList(),
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}