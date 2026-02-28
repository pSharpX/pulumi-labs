using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;

namespace defaultapp.components;

public class DefaultAppComponent: ComponentResource
{
    public Output<string> Endpoint { get; private set; }

    public DefaultAppComponent(string name, DefaultAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:DefaultAppComponent", name, options)
    {
        var workspace = new Workspace("OneBank_OperationalInsightsWorkspace", new WorkspaceArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            WorkspaceName = args.ParentName.Apply(resourceName => $"{resourceName}-cluster-logws-{args.Environment}"),
            Sku = new WorkspaceSkuArgs()
            {
                Name = WorkspaceSkuNameEnum.PerGB2018,
            },
            RetentionInDays = 30,
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = this });
        
        var managedEnvironment = new ManagedEnvironment("OneBank_ManagedEnvironment", new ManagedEnvironmentArgs
        {
            EnvironmentName = args.ParentName.Apply(environmentName => $"{environmentName}-cluster-{args.Environment}"),
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AppLogsConfiguration = new AppLogsConfigurationArgs()
            {
                Destination = "log-analytics",
                LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
                {
                    CustomerId = workspace.CustomerId,
                    SharedKey = OneBankHelper.GetWorkspaceSharedKeys(args.ResourceGroupName, workspace.Name)
                        .Apply(key => key.PrimarySharedKey!)
                },
            },
            VnetConfiguration = new VnetConfigurationArgs
            {
                Internal = false
            },
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent =  this });
        
        var containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            ContainerAppName = args.Name.Apply(resourceName => $"{resourceName}-app-{args.Environment}"),
            EnvironmentId =  managedEnvironment.Id,
            Configuration = new ConfigurationArgs
            {
                Ingress = new IngressArgs
                {
                    AllowInsecure = false,
                    External =  true,
                    TargetPort = 80
                }
            },
            Template = new TemplateArgs
            {
                RevisionSuffix = args.Name.Apply(resourceName => $"{resourceName}-app-{args.Environment}"),
                Containers = new[]
                {
                    new ContainerArgs
                    {
                        Image = Output.Format($"{args.Image}:{args.ImageVersion}"),
                        Name = args.Name.Apply(resourceName => $"{resourceName}-app-{args.Environment}"),
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = 0.5,
                            Memory = "1.0Gi"
                        }
                    }
                },
                Scale = new ScaleArgs
                {
                    MinReplicas = 1,
                    MaxReplicas = 3,
                    Rules = new []
                    {
                        new ScaleRuleArgs
                        {
                            Name = "http-rules",
                            Http = new HttpScaleRuleArgs
                            {
                                Metadata =
                                {
                                    {"concurrentRequests", "10"}
                                }
                            }
                            
                        }
                    }
                }
            },
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = this });

        Endpoint = containerApp.Configuration.Apply(configuration => $"https://{configuration?.Ingress?.Fqdn!}");
        
        RegisterOutputs(new Dictionary<string, object?>
        {
            {"Endpoint", Endpoint}
        });
    }
}