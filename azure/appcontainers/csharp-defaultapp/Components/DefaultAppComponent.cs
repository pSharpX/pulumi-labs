using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using defaultapp.Factories;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;

namespace defaultapp.components;

public class DefaultAppComponent: ComponentResource
{
    private readonly Workspace _workspace;
    private readonly ManagedEnvironment _managedEnvironment;
    
    private VirtualNetwork? _virtualNetwork;
    private Vault? _vault;
    private ImmutableList<Secret>? _secrets;
    
    public Output<string> Endpoint { get; }

    public DefaultAppComponent(string name, DefaultAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:DefaultAppComponent", name, options)
    {
        _workspace = new Workspace("OneBank_OperationalInsightsWorkspace", new WorkspaceArgs
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

        _virtualNetwork = args is { Private: true, SubnetId: null } ? VirtualNetworkFactory.Create(new CreateVirtualNetworkArgs
            {
                Name = $"{args.ParentName}-vnet-{args.Environment}", 
                ResourceGroupName = args.ResourceGroupName, 
                Location = args.Location!,
                AddressPrefixes = args.AddressPrefixes!,
                SubnetAddressPrefixes = args.SubnetAddressPrefixes!,
                Tags = args.Tags,
                Parent = this
            })
            : null;
        var subnetId = _virtualNetwork?.Subnets.Apply(subnets => subnets[0].Id);
        
        _managedEnvironment = new ManagedEnvironment("OneBank_ManagedEnvironment", new ManagedEnvironmentArgs
        {
            EnvironmentName = args.ParentName.Apply(environmentName => $"{environmentName}-cluster-{args.Environment}"),
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AppLogsConfiguration = new AppLogsConfigurationArgs()
            {
                Destination = "log-analytics",
                LogAnalyticsConfiguration = new LogAnalyticsConfigurationArgs
                {
                    CustomerId = _workspace.CustomerId,
                    SharedKey = OneBankHelper.GetWorkspaceSharedKeys(args.ResourceGroupName, _workspace.Name)
                        .Apply(key => key.PrimarySharedKey!)
                },
            },
            VnetConfiguration = args.Private ? 
                new VnetConfigurationArgs
                {
                    Internal = args.Private,
                    InfrastructureSubnetId =  args.SubnetId is not null ? args.SubnetId:  subnetId!
                } : new VnetConfigurationArgs
                {
                    Internal = args.Private,
                },
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent =  this });

        if (!args.Secrets.IsEmpty)
        {
            _vault = CreateVaultFactory.Create(new CreateVaultArgs
            {
                Name = $"{args.ParentName}-vault-{args.Environment}",
                TenantId = GetClientConfig.Invoke().Apply(config => config.TenantId),
                Location = args.Location,
                ResourceGroupName = args.ResourceGroupName,
                Parent = this,
                Tags = args.Tags,
            });

            _secrets = args.Secrets.Select(secret => CreateSecretFactory.Create(new CreateSecretArgs
                {
                    VaultName = _vault.Name,
                    Name = secret.Item1,
                    Value = secret.Item2,
                    Tags = args.Tags,
                    ResourceGroupName = args.ResourceGroupName,
                    Location = null,
                }))
                .ToImmutableList();
        }
        
        var containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            ContainerAppName = args.Name.Apply(resourceName => $"{resourceName}-app-{args.Environment}"),
            EnvironmentId =  _managedEnvironment.Id,
            Configuration = new ConfigurationArgs
            {
                Ingress = new IngressArgs
                {
                    AllowInsecure = false,
                    External =  args.External,
                    TargetPort = args.Port,
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
                            Cpu = args.TotalCpu,
                            Memory = args.TotalMemory,
                        }
                    }
                },
                Scale = args.EnableScaling.Apply(scaling => scaling ? new ScaleArgs
                {
                    MinReplicas = args.MinInstances,
                    MaxReplicas = args.MaxInstances,
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
                }: null)! 
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