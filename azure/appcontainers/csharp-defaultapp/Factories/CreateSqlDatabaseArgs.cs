using Pulumi;

namespace defaultapp.Factories;

public class CreateSqlDatabaseArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public required Input<string> ServerName { get; init; }
    public required Input<string> LicenseType { get; init; } = "LicenseIncluded";
    public required Input<string> SampleName { get; init; } = "onebank";
    public required Input<int> SkuCapacity { get; init; } = 2;
    public required Input<string> SkuFamily { get; init; } = "Gen5";
    public required Input<string> SkuName { get; init; } = "GP_S";
}