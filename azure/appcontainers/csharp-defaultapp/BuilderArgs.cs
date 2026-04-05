using Pulumi;
using Pulumi.AzureNative.Resources;
using Resource = Pulumi.Resource;

namespace defaultapp;

public class BuilderArgs
{
    public required Config Config { get; set; }
    public InputList<Resource>? DependsOn { get; set; }
    public required ResourceGroup ResourceGroup { get; set; }
}