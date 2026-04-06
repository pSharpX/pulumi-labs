namespace defaultapp.Factories;

public class CreateLogAnalyticsWorkspaceArgs: CreateResourceArgs
{
    public int RetentionInDays { get; set; }
    public string SkuName { get; set; } = "PerGB2018"; // PerGB2018, Free, Standard, Premium, 
}