using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using SubnetArgs = Pulumi.AzureNative.Network.Inputs.SubnetArgs;

namespace defaultapp.Factories;

public static class PrivateEndpointFactory
{
    public static PrivateEndpoint Create(CreatePrivateEndpointArgs args)
    {
        var privateEndpointArgs = new PrivateEndpointArgs
        {
            PrivateEndpointName = args.Name,
            Subnet = new SubnetArgs
            {
                Id = args.SubnetId,
            },
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Tags = args.Tags!,
            PrivateLinkServiceConnections = args.PrivateLinkServiceId?
                .Apply(ids => ids.Select(id => new PrivateLinkServiceConnectionArgs
                {
                    Name = Output.Format($"{args.Name}-connection"),
                    PrivateLinkServiceId = id,
                    GroupIds =
                    [
                        args.GroupId
                    ]
                }))!,

            IpConfigurations = args.PrivateIpAddresses?.Apply(privateIpAddresses =>
                privateIpAddresses.Select(ip => new PrivateEndpointIPConfigurationArgs
                {
                    PrivateIPAddress = ip,
                }))!
        };
        
        return new PrivateEndpoint($"OneBank_PrivateEndpoint_{args.Alias}", privateEndpointArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}