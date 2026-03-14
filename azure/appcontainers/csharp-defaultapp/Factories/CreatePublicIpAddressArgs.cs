namespace defaultapp.Factories;

public class CreatePublicIpAddressArgs: CreateResourceArgs
{
    public required string DnsNameLabel { get; init;  }
    public string IpVersion { get; init; } = "IPv4"; // IPv6
    public string IpAllocationMethod { get; init; } = "Dynamic"; // Static 
    public string SkuName { get; init; } = "Basic"; // Standard
    public string SkuTier { get; init; } = "Global"; // Regional
    public string DeleteOption { get; init; } = "Detach"; // Delete
    public int IdleTimeoutInMinutes { get; init; } = 10;
}