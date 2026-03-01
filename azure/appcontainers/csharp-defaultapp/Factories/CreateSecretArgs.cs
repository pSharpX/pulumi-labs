using Pulumi;

namespace defaultapp.Factories;

public class CreateSecretArgs: CreateResourceArgs
{
    public required Input<string> Value { get; init; }
    public required Input<string> VaultName { get; init; }
    public Input<bool> Enabled { get; init; } = true;
    public Input<int>? Expires { get; init; }
    public Input<int>? NotBefore { get; init; }
    public Input<string>? ContentType { get; init; }
}