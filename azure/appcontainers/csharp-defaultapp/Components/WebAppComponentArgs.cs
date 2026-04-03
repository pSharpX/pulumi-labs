using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;

namespace defaultapp.components;

public class WebAppComponentArgs: AppComponentArgs
{
    public string? Image { get; init; }
    public string ImageVersion { get; init; } = "latest";
    public string? RepositoryUrl { get; init; }
    public string Branch { get; init; } = "main";
    public string? Runtime { get; init; }
    public string? StartupCommandLine { get; init; }
    public IImmutableList<(string, string, string)> Secrets { get; init; } = ImmutableList.Create< (string, string, string)>();
    public ImmutableList<(string, string, string)> AppConfig { get; init; } = ImmutableList.Create< (string, string, string)>();
    public ImmutableList<(string, string)> AppSettings { get; init; } = ImmutableList.Create< (string, string)>();
    public List<string> AllowedOrigins { get; init; } = ["*"];
    public string HealthCheckPath { get; set; } = "/";
}