using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using defaultapp.Factories;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.Random;
using ManagedServiceIdentityArgs = Pulumi.AzureNative.App.Inputs.ManagedServiceIdentityArgs;
using ManagedServiceIdentityType = Pulumi.AzureNative.App.ManagedServiceIdentityType;
using SecretArgs = Pulumi.AzureNative.App.Inputs.SecretArgs;

namespace defaultapp.components;

public class DefaultAppComponent: ComponentResource
{
    private readonly Workspace _workspace;
    private readonly ManagedEnvironment _managedEnvironment;
    
    private VirtualNetwork? _virtualNetwork;
    private UserAssignedIdentity? _managedIdentity;
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
            WorkspaceName = Output.Format($"{args.ParentName}-cluster-logws-{args.Environment}"),
            Sku = new WorkspaceSkuArgs()
            {
                Name = WorkspaceSkuNameEnum.PerGB2018,
            },
            RetentionInDays = 30,
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = this });

        _virtualNetwork = args is { Private: true, SubnetId: null } ? VirtualNetworkFactory.Create(new CreateVirtualNetworkArgs
            {
                Name = Output.Format($"{args.ParentName}-vnet-{args.Environment}"), 
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
            EnvironmentName = Output.Format($"{args.ParentName}-cluster-{args.Environment}"),
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

        ManagedServiceIdentityArgs? identity = null;
        List<SecretArgs> secretListArgs = [];
        if (!args.Secrets.IsEmpty)
        {
            _managedIdentity = UserAssignedIdentityFactory.Create(new CreateUserAssignedIdentityArgs
            {
                Name = Output.Format($"{args.ParentName}-managed-identity-{args.Environment}"),
                Location = args.Location,
                ResourceGroupName = args.ResourceGroupName,
                Parent = this,
                Tags = args.Tags
            });
            identity = new ManagedServiceIdentityArgs
            {
                Type = ManagedServiceIdentityType.UserAssigned,
                UserAssignedIdentities = new[] { _managedIdentity.Id }
            };
                
            _vault = VaultFactory.Create(new CreateVaultArgs
            {
                Name = Output.Format($"{args.ParentName}-vault-{args.Environment}"),
                TenantId = args.TenantId,
                Location = args.Location,
                ResourceGroupName = args.ResourceGroupName,
                Parent = this,
                Tags = args.Tags,
            });

            var kvSecretsUserRole = OneBankHelper.GetRoleDefinition(BuiltInRole.KeyVaultSecretsUser, _vault.Id).Apply(rd => rd.Id);
            var kvAdminRole = OneBankHelper.GetRoleDefinition(BuiltInRole.KeyVaultAdministrator, _vault.Id).Apply(rd => rd.Id);
            var appRoleAssignment = RoleAssignmentFactory.Create(new CreateRoleAssignmentArgs
            {
                Name = new RandomUuid("OneBank_RoleAssignment_App_Vault_UUID", new RandomUuidArgs { Keepers =
                    {
                        { "ResourceGroupName", args.ResourceGroupName }, 
                        { "ManagedIdentityId", _managedIdentity.Id },
                        { "RoleDefinitionId", kvSecretsUserRole }
                    }
                }, new CustomResourceOptions { Parent = this }).Result,
                Alias = "App_Vault",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                RoleDefinitionId = kvSecretsUserRole,
                PrincipalId = _managedIdentity.PrincipalId,
                Scope = _vault.Id,
                Parent = this,
                Tags = args.Tags
            });
            var adminRoleAssignment = RoleAssignmentFactory.Create(new CreateRoleAssignmentArgs
            {
                Name = new RandomUuid("OneBank_RoleAssignment_Admin_Vault_UUID", new RandomUuidArgs { Keepers =
                    {
                        { "ResourceGroupName", args.ResourceGroupName }, 
                        { "ManagedIdentityId", args.ObjectId },
                        { "RoleDefinitionId", kvAdminRole }
                    }
                }, new CustomResourceOptions { Parent = this }).Result,
                Alias = "Admin_Vault",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                RoleDefinitionId = kvAdminRole,
                PrincipalId = args.ObjectId,
                PrincipalType = PrincipalType.User,
                Scope = _vault.Id,
                Parent = this,
                Tags = args.Tags
            });

            _secrets = args.Secrets.Select(secret => SecretFactory.Create(new CreateSecretArgs
                {
                    VaultName = _vault.Name,
                    Name = secret.Item1,
                    Alias = secret.Item2,
                    Value = secret.Item3,
                    Tags = args.Tags,
                    ResourceGroupName = args.ResourceGroupName,
                    Location = null,
                }))
                .ToImmutableList();

            secretListArgs = _secrets.Select(secret => new SecretArgs
            {
                Identity = _managedIdentity.Id,
                KeyVaultUrl = secret.Properties.Apply(props => props.SecretUri),
                Name = secret.Name
            }).ToList();
        }

        List<ContainerAppProbeArgs> probes = [];
        if (args.EnableProbes)
        {
            probes =
            [
                new ContainerAppProbeArgs
                {
                    FailureThreshold = 3,
                    HttpGet = new ContainerAppProbeHttpGetArgs
                    {
                        Port = args.Port,
                        Path = args.Path
                    },
                    InitialDelaySeconds = args.InitialDelaySeconds,
                    PeriodSeconds = args.PeriodSeconds,
                    SuccessThreshold = 1,
                    Type = Pulumi.AzureNative.App.Type.Liveness
                }
            ];
        }
        
        var containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            ContainerAppName = Output.Format($"{args.Name}-app-{args.Environment}"),
            EnvironmentId =  _managedEnvironment.Id,
            Identity = identity!,
            Configuration = new ConfigurationArgs
            {
                Ingress = new IngressArgs
                {
                    AllowInsecure = false,
                    External =  args.External,
                    TargetPort = args.Port,
                    CorsPolicy = new CorsPolicyArgs
                    {
                        AllowedOrigins = args.AllowedOrigins,
                        AllowedHeaders = args.AllowedHeaders,
                        AllowedMethods = args.AllowedMethods
                    }
                },
                Secrets = secretListArgs,
            },
            Template = new TemplateArgs
            {
                RevisionSuffix = Output.Format($"{args.Name}-app-{args.Environment}"),
                Containers = new[]
                {
                    new ContainerArgs
                    {
                        Image = Output.Format($"{args.Image}:{args.ImageVersion}"),
                        Name = Output.Format($"{args.Name}-app-{args.Environment}"),
                        Resources = new ContainerResourcesArgs
                        {
                            Cpu = args.TotalCpu,
                            Memory = args.TotalMemory,
                        },
                        Probes = probes
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