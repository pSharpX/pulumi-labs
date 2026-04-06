using Pulumi;

namespace defaultapp.Factories;

public class CreateApplicationInsightsArgs: CreateResourceArgs
{
    // Specifies the type of Application Insights to create. Valid values are ios for iOS, java for Java web, MobileCenter for App Center, Node.JS for Node.js, other for General, phone for Windows Phone, store for Windows Store and web for ASP.NET.
    public string Kind { get; set; } = "web"; // web, java, ios, MobileCenter, phone, store, Node.JS, other
    public string ApplicationType { get; set; } = "web"; // web, other 
    public required Input<string> WorkspaceId { get; set; }
    public bool DisableIpMasking { get; set; }
    public int RetentionInDays { get; set; } = 30;
}