using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Pulumi;
using Pulumi.AzureNative.Sql;

namespace defaultapp.components;

public class DefaultAppComponentArgs: AppComponentArgs
{
    public required bool Private { get; init; }
    public required string Image { get; init; }
    public Input<string> ImageVersion { get; init; } = "latest";
    public required bool External { get; init; } = false;
    public Input<int> Port { get; init; } = 80;
    public required Input<double> TotalCpu { get; init; }
    public required Input<string> TotalMemory { get; init; }
    public Input<bool> EnableScaling { get; init; } = false;
    public Input<int> MinInstances { get; init; } = 1;   
    public Input<int> MaxInstances { get; init; } = 1;
    public VirtualNetworkConfig? VirtualNetworkConfig { get; init; }
    public ImmutableList<(string, string, string)> Secrets { get; init; } = ImmutableList.Create< (string, string, string)>();
    public ImmutableList<(string, string, string)> AppConfig { get; init; } = ImmutableList.Create< (string, string, string)>();
    public List<string> AllowedOrigins { get; init; } = ["*"];
    public List<string> AllowedHeaders { get; init; } = ["*"];
    public List<string> AllowedMethods { get; init; } = ["*"];
    public bool EnableProbes { get; init; } = false;
    public Input<string> Path { get; set; } = "/";
    public Input<int> InitialDelaySeconds { get; set; } = 3;
    public Input<int> PeriodSeconds { get; set; } = 3;
    public DatabaseConfig? DatabaseConfig { get; init; }
}

public class DatabaseConfig
{
    [JsonPropertyName("databaseName")]
    public string DatabaseName { get; set; } = "";
    [JsonPropertyName("username")]
    public string Username { get; set; } = "";
    [JsonPropertyName("password")]
    public string Password { get; set; } = "";
}

public class VirtualNetworkConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("subnetId")]
    public string SubnetId { get; set; } = "";
    [JsonPropertyName("addressPrefixes")]
    public List<string> AddressPrefixes { get; set; } = new();
    
    [JsonPropertyName("subnets")]
    public List<SubnetConfig> Subnets { get; set; } = new();
}

public class SubnetConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("alias")]
    public string Alias { get; set; } = "";
    [JsonPropertyName("addressPrefixes")]
    public List<string>? AddressPrefixes { get; set; } = new();
    [JsonPropertyName("delegations")]
    public List<NetworkDelegationConfig> Delegations { get; set; } = new();
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
    
}

public class NetworkDelegationConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("serviceName")]
    public string ServiceName { get; set; } = "";
}