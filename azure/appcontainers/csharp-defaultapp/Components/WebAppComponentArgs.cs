using System.Collections.Generic;
using Pulumi;

namespace defaultapp.components;

public class WebAppComponentArgs: AppComponentArgs
{
    public string? Image { get; init; }
    public string ImageVersion { get; init; } = "latest";
    public List<string> AllowedOrigins { get; init; } = ["*"];
}