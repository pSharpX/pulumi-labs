using Pulumi;

namespace defaultapp.Factories;

public class CreateAppServicePlanArgs: CreateResourceArgs
{
    public string Kind { get; init; } = "app,linux"; // 'app' | 'app,linux' | 'app,linux,container' | 'hyperV' | 'app,container,windows' | 'app,linux,kubernetes' | 'app,linux,container,kubernetes' | 'functionapp' | 'functionapp,linux' | 'functionapp,linux,container,kubernetes' | 'functionapp,linux,kubernetes'
    public string SkuName { get; init; } = "F1";
    public bool Isolated { get; init; } = false;
    public Input<string>? EnvironmentId { get; init; }
    public bool Reserved => Kind.Contains("linux");
    public bool EnableScaling { get; init; }
    public Input<int> MinInstances { get; init; } = 1;   
    public Input<int> MaxInstances { get; init; } = 1;
}