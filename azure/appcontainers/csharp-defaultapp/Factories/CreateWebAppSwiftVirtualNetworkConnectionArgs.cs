using Pulumi;

namespace defaultapp.Factories;

public class CreateWebAppSwiftVirtualNetworkConnectionArgs: CreateResourceArgs
{
    public required Input<string> SubnetId { get; set; }
}