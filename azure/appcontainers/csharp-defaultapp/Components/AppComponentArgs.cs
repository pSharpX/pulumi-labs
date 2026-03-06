using Pulumi;

namespace defaultapp.components;

public abstract class AppComponentArgs
{
    public required Input<string> ClientId { get; init; }
    public required Input<string> ObjectId { get; init; }
    public required Input<string> TenantId { get; init; }
    public required Input<string> SubscriptionId { get; init; }
    public required Input<string> Name { get; init; }
    public required Input<string> ParentName { get; init; }
    public required Input<string> ResourceGroupName { get; init; }
    public Input<string>? Location { get; init; }
    public string Environment { get; init; } = "dev";
    public bool EnableVault { get; init; }
    public string? VaultName { get; init; }
    public bool EnableConfigStore { get; init; }
    public string? ConfigStoreName { get; init; }
    public bool EnableStorage { get; init; }
    public string? StorageAccountName { get; init; }
    public bool EnableRegistry { get; init; }
    public string? RegistryName { get; init; }
    public bool EnableEncryption { get; init; }
    public string? EncryptionKeyName { get; init; }
}