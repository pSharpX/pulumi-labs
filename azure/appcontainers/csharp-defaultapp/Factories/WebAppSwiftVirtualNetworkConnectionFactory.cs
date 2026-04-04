using Pulumi;
using Pulumi.AzureNative.Web;

namespace defaultapp.Factories;

public static class WebAppSwiftVirtualNetworkConnectionFactory
{
    public static WebAppSwiftVirtualNetworkConnection Create(CreateWebAppSwiftVirtualNetworkConnectionArgs args)
    {
        return new WebAppSwiftVirtualNetworkConnection("OneBank_WebAppSwiftVirtualNetworkConnection", new WebAppSwiftVirtualNetworkConnectionArgs
        {
            Name = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            SubnetResourceId = args.SubnetId,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}