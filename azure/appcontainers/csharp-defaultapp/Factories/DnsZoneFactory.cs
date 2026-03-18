using Pulumi;
using Pulumi.AzureNative.Dns;

namespace defaultapp.Factories;

public static class DnsZoneFactory
{
    public static Zone Create(CreateDnsZoneArgs args)
    {
        return new Zone($"OneBank_DnsZone_{args.Alias}", new ZoneArgs
        {
            ZoneName = args.Name,
            ZoneType =  args.ZoneType!,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    } 
}