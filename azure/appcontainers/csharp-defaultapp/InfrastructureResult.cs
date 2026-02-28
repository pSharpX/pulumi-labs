using Pulumi;
using Pulumi.AzureNative.KeyVault;

namespace defaultapp;

public class InfrastructureResult
{
    public ComponentResource? Application { get; init; }
    public Vault? Vault { get; init; }
    public ComponentResource? Database { get; init; }
}