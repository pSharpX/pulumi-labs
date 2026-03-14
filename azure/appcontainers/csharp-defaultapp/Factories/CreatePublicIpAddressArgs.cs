using Pulumi;

namespace defaultapp.Factories;

public class CreatePublicIpAddressArgs: CreateResourceArgs
{
    public required string Alias { get; init;  }
    public required Input<string> DnsNameLabel { get; init;  }
    public string IpVersion { get; init; } = "IPv4"; // IPv6
    public string IpAllocationMethod { get; init; } = "Dynamic"; // Static 
    public string SkuName { get; init; } = "Basic"; // Basic, Standard
    public string SkuTier { get; init; } = "Regional"; // Regional, Global
    public string DeleteOption { get; init; } = "Detach"; // Delete
    public int IdleTimeoutInMinutes { get; init; } = 10;
}