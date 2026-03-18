namespace defaultapp.Factories;

public class CreatePrivateLinkServiceArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required string FrontendIpConfigurationId { get; init; }
    public required string SubnetId { get; init; }
    public string PrivateIpAllocationMethod { get; init; } = "Dynamic"; // Static, Dynamic
    public string PrivateIpAddressVersion { get; init; } = "IPv4";
    public string? PrivateIpAddress { get; init; }
}