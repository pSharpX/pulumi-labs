namespace defaultapp.Factories;

public class CreatePrivateEndpointArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public string? PrivateLinkServiceId { get; init; }
    public string? PrivateIpAddress { get; init; }
    public required string SubnetId { get; init; }
}