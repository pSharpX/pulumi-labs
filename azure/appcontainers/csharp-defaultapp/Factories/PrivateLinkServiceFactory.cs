using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using PrivateLinkServiceArgs = Pulumi.AzureNative.Network.PrivateLinkServiceArgs;
using SubnetArgs = Pulumi.AzureNative.Network.Inputs.SubnetArgs;

namespace defaultapp.Factories;

public static class PrivateLinkServiceFactory
{
    public static PrivateLinkService Create(CreatePrivateLinkServiceArgs args)
    {
        return new PrivateLinkService($"OneBank_PrivateLink_{args.Alias}", new PrivateLinkServiceArgs
        {
            ServiceName =  args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            IpConfigurations = [
                new PrivateLinkServiceIpConfigurationArgs
                {
                    Name = Output.Format($"{args.Name}-ipconfig"),
                    PrivateIPAddress = args.PrivateIpAddress!,
                    PrivateIPAllocationMethod = args.PrivateIpAllocationMethod,
                    PrivateIPAddressVersion =  args.PrivateIpAddressVersion,
                    Primary = true,
                    Subnet = new SubnetArgs
                    {
                        Id = args.SubnetId
                    }
                }
            ],
            LoadBalancerFrontendIpConfigurations = [
                new FrontendIPConfigurationArgs
                {
                    Id =  args.FrontendIpConfigurationId
                }
            ],
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}