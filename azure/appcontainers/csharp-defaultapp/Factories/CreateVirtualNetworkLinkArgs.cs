namespace defaultapp.Factories;

public class CreateVirtualNetworkLinkArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required string PrivateZoneName { get; init; }
    public bool RegistrationEnabled { get; init; } = true;
    public required string VirtualNetworkId { get; init; }
    
}