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
        };

        if (!string.IsNullOrEmpty(args.PrivateLinkServiceId))
        {
            privateEndpointArgs.PrivateLinkServiceConnections =
            [
                new PrivateLinkServiceConnectionArgs
                {
                    PrivateLinkServiceId = args.PrivateLinkServiceId
                }
            ];
        }
        if (!string.IsNullOrEmpty(args.PrivateIpAddress))
        {
            privateEndpointArgs.IpConfigurations =
            [
                new PrivateEndpointIPConfigurationArgs
                {
                    PrivateIPAddress =  args.PrivateIpAddress,
                }
            ];
        }
        return new PrivateEndpoint($"OneBank_PrivateEndpoint_{args.Alias}", privateEndpointArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}