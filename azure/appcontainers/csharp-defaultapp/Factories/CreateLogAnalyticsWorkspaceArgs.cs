namespace defaultapp.Factories;

public class CreateLogAnalyticsWorkspaceArgs: CreateResourceArgs
{
    public int RetentionInDays { get; set; } = 30;
    public string SkuName { get; set; } = "PerGB2018"; // PerGB2018, Free, Standard, Premium, 
}