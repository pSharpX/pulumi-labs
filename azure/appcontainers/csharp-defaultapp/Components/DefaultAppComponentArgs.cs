using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;

namespace defaultapp.components;

public class DefaultAppComponentArgs
{
    public required Input<string> Name { get; init; }
    public required Input<string> ParentName { get; init; }
    public required Input<string> ResourceGroupName { get; init; }
    public required Input<string>? Location { get; init; }
    public required bool Private { get; init; }
    public string Environment { get; set; } = "dev";
    public Dictionary<string, string>? Tags { get; init; }
    public required Input<string> Image { get; init; }
    public Input<string> ImageVersion { get; init; } = "latest";
    public required Input<bool> External { get; init; } = false;
    public required Input<int> Port { get; init; }
    public required Input<double> TotalCpu { get; init; }
    public required Input<string> TotalMemory { get; init; }
    public Input<bool> EnableScaling { get; init; } = false;
    public Input<int> MinInstances { get; init; } = 1;   
    public Input<int> MaxInstances { get; init; } = 1;
    public string? SubnetId { get; set; }
    public InputList<string>? AddressPrefixes { get; init; }
    public InputList<string>? SubnetAddressPrefixes { get; init; }
    public ImmutableList<(string, string)> Secrets { get; set; } = ImmutableList.Create< (string, string)>();
}