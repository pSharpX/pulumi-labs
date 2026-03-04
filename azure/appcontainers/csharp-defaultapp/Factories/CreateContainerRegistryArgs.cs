using Pulumi;

namespace defaultapp.Factories;

public class CreateContainerRegistryArgs: CreateResourceArgs
{
    public Input<bool> AdminUserEnabled { get; init; } = false;
    public Input<string> NetworkRuleBypassOptions { get; init; } = "";
    public Input<string> SkuName { get; init; } = "Standard"; // Classic, Basic, Standard, Premium
    public Input<string> PublicNetworkAccess { get; init; } = "Enabled"; 
    public Input<bool> AnonymousPullEnabled { get; init; } = false;
    public bool EncryptionEnabled { get; init; } = false;
    public Input<string>? VaultIdentity { get; init; }
    public Input<string>? VaultKeyIdentifier { get; init; }
    
    //Identity
    //NetworkRuleSet
    //Policies
}