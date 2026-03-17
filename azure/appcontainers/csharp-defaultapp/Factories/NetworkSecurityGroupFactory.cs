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
        return new NetworkSecurityGroup($"OneBank_NetworkSecurityGroup_{args.Alias}", new NetworkSecurityGroupArgs
        {
            NetworkSecurityGroupName =  args.Name,
            SecurityRules = args.SecurityRules?.Select(rule =>
            {
                var securityRule = new SecurityRuleArgs
                {
                    Name = rule.Name,
                    Description = rule.Description!,
                    Access = rule.Access,
                    Direction = rule.Direction,
                    Protocol = rule.Protocol,
                    Priority = rule.Priority,
                };

                if (rule.DestinationAddressPrefix is not null)
                {
                    securityRule.DestinationAddressPrefix = rule.DestinationAddressPrefix;
                }

                if (rule.DestinationAddressPrefixes is not null && rule.DestinationAddressPrefixes.Count > 0)
                {
                    securityRule.DestinationAddressPrefixes = rule.DestinationAddressPrefixes;
                }
                
                if (rule.DestinationApplicationSecurityGroups is not null && rule.DestinationApplicationSecurityGroups.Count > 0)
                {
                    securityRule.DestinationApplicationSecurityGroups = rule.DestinationApplicationSecurityGroups
                        .Select(asp => new ApplicationSecurityGroupArgs
                        {
                            Id = asp,
                            Location = args.Location
                        }).ToList()!;
                }
                
                if (rule.DestinationPortRange is not null)
                {
                    securityRule.DestinationPortRange = rule.DestinationPortRange;
                }

                if (rule.DestinationPortRanges is not null && rule.DestinationPortRanges.Count > 0)
                {
                    securityRule.DestinationPortRanges = rule.DestinationPortRanges;
                }
                
                if (rule.SourceAddressPrefix is not null)
                {
                    securityRule.SourceAddressPrefix = rule.SourceAddressPrefix;
                }

                if (rule.SourceAddressPrefixes is not null && rule.SourceAddressPrefixes.Count > 0)
                {
                    securityRule.SourceAddressPrefixes = rule.SourceAddressPrefixes;
                }
                
                if (rule.SourceApplicationSecurityGroups is not null && rule.SourceApplicationSecurityGroups.Count > 0)
                {
                    securityRule.SourceApplicationSecurityGroups = rule.SourceApplicationSecurityGroups?
                        .Select(asp => new ApplicationSecurityGroupArgs
                        {
                            Id = asp,
                            Location = args.Location
                        }).ToList()!;
                }
                
                if (rule.SourcePortRange is not null)
                {
                    securityRule.SourcePortRange = rule.SourcePortRange;
                }

                if (rule.SourcePortRanges is not null && rule.SourcePortRanges.Count > 0)
                {
                    securityRule.SourcePortRanges = rule.SourcePortRanges;
                }
                
                return securityRule;
            }).ToList()!,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}