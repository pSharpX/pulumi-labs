using System.Collections.Generic;
using Pulumi;

namespace defaultapp.components;

public class DefaultAppComponentArgs
{
    public required Input<string> Name { get; set; }
    public required Input<string> ParentName { get; set; }
    public required Input<string> ResourceGroupName { get; set; }
    public required Input<string>? Location { get; set; }
    public string Environment { get; set; } = "dev";
    public Dictionary<string, string>? Tags { get; set; }
    public required Input<string> Image { get; set; }
    public Input<string> ImageVersion { get; set; } = "latest";
}