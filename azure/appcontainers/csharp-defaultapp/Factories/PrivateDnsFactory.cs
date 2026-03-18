using Pulumi;
using Pulumi.AzureNative.Dns;
using Pulumi.AzureNative.PrivateDns;

namespace defaultapp.Factories;

public static class PrivateDnsFactory
{
    public static PrivateZone Create(CreateDnsZoneArgs args)
    {
        return new PrivateZone($"OneBank_PrivateDnsZone_{args.Alias}", new PrivateZoneArgs
        {
            PrivateZoneName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    } 
}