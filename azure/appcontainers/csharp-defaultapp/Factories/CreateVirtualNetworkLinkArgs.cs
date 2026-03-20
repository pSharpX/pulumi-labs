using Pulumi;

namespace defaultapp.Factories;

public class CreateVirtualNetworkLinkArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required Input<string> PrivateZoneName { get; init; }
    public bool RegistrationEnabled { get; init; } = true;
    public required Input<string> VirtualNetworkId { get; init; }
    
}