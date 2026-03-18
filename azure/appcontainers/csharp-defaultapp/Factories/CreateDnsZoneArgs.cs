using Pulumi.AzureNative.Dns;

namespace defaultapp.Factories;

public class CreateDnsZoneArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public ZoneType ZoneType { get; init; } = ZoneType.Private; // Public, Private
}