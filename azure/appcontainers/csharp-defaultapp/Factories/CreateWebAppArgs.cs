using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateWebAppArgs: CreateResourceArgs
{
    public required string Alias { get; set; }
    public string Kind { get; set; } = "app";
    public required Input<string> ServicePlanId { get; set; }
    public required InputList<string> ManagedIdentities { get; set; }
    public bool Enabled { get; set; } = true;
    public bool IsLinux { get; set; } = true;
    public bool Containerized { get; set; } = true;
    public string? ImageName { get; set; }
    public string ImageTag { get; set; } = "latest";
    public Input<string>? RegistryName { get; set; }
    public Input<string>? RegistryUsername { get; set; }
    public Input<string>? RegistryPassword { get; set; }
    public string? HealthCheckPath { get; set; }
    public string? StartupCommandLine { get; set; }
    public string? Runtime { get; set; }
    public List<string> AllowedOrigins { get; set; } = ["*"];
    public Dictionary<string, Input<string>> AppSettings = new();
    public string PublicNetworkAccess { get; set; } = "Enabled"; // Enabled, Disabled
    public bool HttpsOnly { get; set; } = true;
    public bool WebSocketsEnabled  { get; set; }
    public bool AppInsightsEnabled  { get; set; }
    public string? Stack { get; set; }
    public Input<string>? AppInsightsInstrumentationKey { get; set; }
    public Input<string>? AppInsightsConnectionString { get; set; }
}