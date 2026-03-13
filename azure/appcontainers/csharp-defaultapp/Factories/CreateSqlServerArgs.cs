using Pulumi;

namespace defaultapp.Factories;

public class CreateSqlServerArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public Input<string> PublicNetworkAccess { get; init; } = "Enabled"; // 'Enabled' or 'Disabled' or 'SecuredByPerimeter'
    public required Input<string> AdministratorLogin { get; init; } 
    public required Input<string> AdministratorLoginPassword { get; init; } 
    
    public Input<string> IsIPv6Enabled { get; init; } = "Disabled";
    public Input<string> MinimalTlsVersion { get; init; } = "1.2"; // 'None', 1.0', '1.1', '1.2', '1.3'
    public Input<string> Version { get; init; } = "12.0";
}