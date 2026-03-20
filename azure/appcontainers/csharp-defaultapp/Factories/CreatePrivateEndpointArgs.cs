using Pulumi;

namespace defaultapp.Factories;

public class CreatePrivateEndpointArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required Input<string> SubnetId { get; init; }
    public InputList<string>? PrivateLinkServiceId { get; init; } = [];
    public string? GroupId { get; init; }
    public InputList<string>? PrivateIpAddresses { get; init; } = [];
}