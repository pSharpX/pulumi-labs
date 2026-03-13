using Pulumi;

namespace defaultapp.Factories;

public class CreateSqlDatabaseArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required Input<string> ServerName { get; init; }
    public Input<string> LicenseType { get; init; } = "LicenseIncluded";
    public Input<string> SampleName { get; init; } = "onebank";
    public Input<int> SkuCapacity { get; init; } = 2;
    public Input<string> SkuFamily { get; init; } = "Gen5";
    public Input<string> SkuName { get; init; } = "GP_S";
}