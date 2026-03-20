using Pulumi;

namespace defaultapp.Factories;

public class CreatePrivateLinkServiceArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required Input<string> FrontendIpConfigurationId { get; init; }
    public required Input<string> SubnetId { get; init; }
    public string PrivateIpAllocationMethod { get; init; } = "Dynamic"; // Static, Dynamic
    public string PrivateIpAddressVersion { get; init; } = "IPv4";
    public Input<string>? PrivateIpAddress { get; init; }
}