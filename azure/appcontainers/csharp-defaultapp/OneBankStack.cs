using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;

public class OneBankStack: Stack 
{
    private readonly ResourceGroup _resourceGroup;
    private readonly Workspace _workspace;
    private readonly ManagedEnvironment _managedEnvironment;
    private readonly ContainerApp _container;
    
    public OneBankStack()
    {
        var config = new Config();
        var resourceGroupName = config.Require("resourceGroupName");
        var workspaceName = config.Require("workspaceName");
        var managedEnvironmentName = config.Require("managedEnvironmentName");
        var isPrivate = config.RequireBoolean("private");
        var applicationName = config.Require("applicationName");
        var applicationId = config.Require("applicationId");
        var applicationContainerName = config.Require("applicationContainerName");
        var imageName = config.Require("imageName");
        var tags = config.RequireObject<Dictionary<string, string>>("tags");
        
        _resourceGroup = CreateResourceGroup(resourceGroupName, tags);
        _workspace =  CreateWorkspace(workspaceName, tags);
        _managedEnvironment = CreateManagedEnvironment(managedEnvironmentName, tags, isPrivate);
        _container = CreateContainerApp(applicationId ,applicationName, applicationContainerName, imageName, tags);
        
        ResourceGroupId = _resourceGroup.Id;
        ManagedEnvironmentId = _managedEnvironment.Id;
        OperationalInsightsWorkspaceId = _workspace.Id;
        ContainerAppId = _container.Id;
        ContainerAppFqdn = _container.Configuration.Apply(configuration => configuration?.Ingress?.Fqdn!);
    }

    private ResourceGroup CreateResourceGroup(string resourceName,  Dictionary<string, string> tags)
    {
        var resourceGroup = new ResourceGroup("TeamLvX_rg", new ResourceGroupArgs
        {
            ResourceGroupName = resourceName,
            Tags = tags
        });
        return resourceGroup;
    }

    private Workspace CreateWorkspace(string resourceName, Dictionary<string, string> tags)
    {
        var workspace = new Workspace("OneBank_OperationalInsightsWorkspace", new WorkspaceArgs
        {
            ResourceGroupName = _resourceGroup.Name,
            WorkspaceName = resourceName,
            Sku = new WorkspaceSkuArgs
            {
                Name = WorkspaceSkuNameEnum.PerGB2018,
            },
            RetentionInDays = 30,
            Tags = tags
        });
        return workspace;
    }

    private ManagedEnvironment CreateManagedEnvironment(string resourceName, Dictionary<string, string> tags, bool isPrivate)
    {
        var managedEnvironment = new ManagedEnvironment("OneBank_ManagedEnvironment", new ManagedEnvironmentArgs
        {
            EnvironmentName = resourceName,
            ResourceGroupName = _resourceGroup.Name,
            AppLogsConfiguration = new AppLogsConfigurationArgs
            {
                Destination = "log-analytics",
                LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
                {
                    CustomerId = _workspace.CustomerId,
                    SharedKey = OneBankHelper.GetWorkspaceSharedKeys(_resourceGroup.Name, _workspace.Name)
                        .Apply(key => key.PrimarySharedKey!)
                },
            },
            VnetConfiguration = new VnetConfigurationArgs
            {
                Internal = isPrivate
            },
            Tags = tags
        });
        return managedEnvironment;
    }

    private ContainerApp CreateContainerApp(string applicationId, string applicationName, string applicationContainerName, string imageName, Dictionary<string, string> tags)
    {
        var containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
        {
            ResourceGroupName = _resourceGroup.Name,
            ContainerAppName = applicationName,
            EnvironmentId =  _managedEnvironment.Id,
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
                RevisionSuffix = applicationId,
                Containers = new[]
                {
                    new ContainerArgs
                    {
                        Image = imageName,
                        Name = applicationContainerName,
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
            Tags = tags
        });
        return containerApp;
    }

    [Output]
    public Output<string> ResourceGroupId { get; private set; }
    [Output]
    public Output<string> OperationalInsightsWorkspaceId { get; private set; }
    [Output]
    public Output<string> ManagedEnvironmentId { get; private set; }
    [Output]
    public Output<string> ContainerAppId { get; private set; }
    [Output]
    public Output<string> ContainerAppFqdn { get; private set; }
}