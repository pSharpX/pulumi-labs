using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;

namespace defaultapp.components;

public class DefaultAppComponentArgs: AppComponentArgs
{
    public required bool Private { get; init; }
    public string Environment { get; set; } = "dev";
    public Dictionary<string, string>? Tags { get; init; }
    public required Input<string> Image { get; init; }
    public Input<string> ImageVersion { get; init; } = "latest";
    public required Input<bool> External { get; init; } = false;
    public Input<int> Port { get; init; } = 80;
    public required Input<double> TotalCpu { get; init; }
    public required Input<string> TotalMemory { get; init; }
    public Input<bool> EnableScaling { get; init; } = false;
    public Input<int> MinInstances { get; init; } = 1;   
    public Input<int> MaxInstances { get; init; } = 1;
    public string? SubnetId { get; set; }
    public InputList<string>? AddressPrefixes { get; init; }
    public InputList<string>? SubnetAddressPrefixes { get; init; }
    public ImmutableList<(string, string, string)> Secrets { get; set; } = ImmutableList.Create< (string, string, string)>();
    public List<string> AllowedOrigins { get; set; } = ["*"];
    public List<string> AllowedHeaders { get; set; } = ["*"];
    public List<string> AllowedMethods { get; set; } = ["*"];
    public bool EnableProbes { get; set; } = false;
    public Input<string> Path { get; set; } = "/";
    public Input<int> InitialDelaySeconds { get; set; } = 3;
    public Input<int> PeriodSeconds { get; set; } = 3;
}