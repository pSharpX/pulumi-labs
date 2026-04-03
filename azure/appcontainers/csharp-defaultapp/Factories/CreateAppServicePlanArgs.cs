namespace defaultapp.Factories;

public class CreateAppServicePlanArgs: CreateResourceArgs
{
    public string Kind { get; init; } = "app"; // 'app' | 'app,linux' | 'app,linux,container' | 'hyperV' | 'app,container,windows' | 'app,linux,kubernetes' | 'app,linux,container,kubernetes' | 'functionapp' | 'functionapp,linux' | 'functionapp,linux,container,kubernetes' | 'functionapp,linux,kubernetes'
    public string SkuName { get; init; } = "F1"; // 'F1' | 'D1', 'B1' | 'B2' | 'B3', 'S1' | 'S2' | 'S3'
}