using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative.KeyVault;

namespace defaultapp.Factories;

public class CreateVaultArgs: CreateResourceArgs
{
    public required Input<string> TenantId { get; init; }
    public Input<bool> EnableRbacAuthorization { get; init; } = true;
    public Input<bool> EnabledForTemplateDeployment { get; init; } = true;
    public Input<bool> EnableSoftDelete { get; init; } = false;
    public Input<int> SoftDeleteRetentionInDays { get; init; } = 7;
    public Input<string> PublicNetworkAccess { get; init; } = "Enabled";
    public Input<SkuName> SkuName { get; init; } = Pulumi.AzureNative.KeyVault.SkuName.Standard;
    public Input<SkuFamily> SkuFamily { get; init; } = Pulumi.AzureNative.KeyVault.SkuFamily.A;
    public bool EnableNetworkAcl { get; init; } = false;
    public NetworkAclRuleSetArgs NetworkAclRuleSet { get; set; } = new ();
}

public class NetworkAclRuleSetArgs
{
    public Input<NetworkRuleBypassOptions> Bypass { get; set; } = NetworkRuleBypassOptions.AzureServices;
    public Input<NetworkRuleAction> DefaultAction { get; set; } = NetworkRuleAction.Allow;
    public List<string>? IpRules { get; set; }
    public List<string>? VirtualNetworkRules { get; set; }
}