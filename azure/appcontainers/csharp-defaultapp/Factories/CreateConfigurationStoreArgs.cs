using Pulumi;

namespace defaultapp.Factories;

public class CreateConfigurationStoreArgs: CreateResourceArgs
{
    public Input<bool> DisableLocalAuth { get; init; } = true;
    public Input<bool> EnablePurgeProtection { get; init; } = false;
    public Input<string> PublicNetworkAccess { get; init; } = "Enabled";
    public Input<int> SoftDeleteRetentionInDays { get; init; } = 7;
    public Input<string> SkuName { get; init; } = "Standard";
    public bool EncryptionEnabled { get; init; } = false;
    public Input<string>? VaultIdentity { get; init; }
    public Input<string>? VaultKeyIdentifier { get; init; }
}