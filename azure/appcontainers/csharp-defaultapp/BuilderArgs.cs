using Pulumi;
using Pulumi.AzureNative.Resources;
using Resource = Pulumi.Resource;

namespace defaultapp;

public class BuilderArgs
{
    public Config Config { get; set; }
    public InputList<Resource>? DependsOn { get; set; }
    public ResourceGroup ResourceGroup { get; set; }
}