using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using defaultapp.Factories;
using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Storage;
using Pulumi.Random;
using ManagedServiceIdentityArgs = Pulumi.AzureNative.App.Inputs.ManagedServiceIdentityArgs;
using ManagedServiceIdentityType = Pulumi.AzureNative.App.ManagedServiceIdentityType;
using SecretArgs = Pulumi.AzureNative.App.Inputs.SecretArgs;

namespace defaultapp.components;

public class DefaultAppComponent: ComponentResource
{
    private readonly UserAssignedIdentity _managedIdentity;
    private readonly Workspace _workspace;
    private readonly ManagedEnvironment _managedEnvironment;
    
    private VirtualNetwork? _virtualNetwork;
    
    private Vault? _vault;
    private Output<GetVaultResult>? _existentVault;
    
    private ConfigurationStore? _configurationStore;
    private Output<GetConfigurationStoreResult>? _existentConfigStore;

    private StorageAccount? _storageAccount;
    private Output<GetStorageAccountResult>? _existentStorageAccount;
    
    private ImmutableList<Secret>? _secrets;
    
    public Output<string> Endpoint { get; }

    public DefaultAppComponent(string name, DefaultAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:DefaultAppComponent", name, options)
    {
        _managedIdentity = UserAssignedIdentityFactory.Create(new CreateUserAssignedIdentityArgs
        {
            Name = Output.Format($"{args.ParentName}-managed-identity-{args.Environment}"),
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            Parent = this,
            Tags = args.Tags
        });
        
        InitializeVault(args);
        InitializeConfigStore(args);
        InitializeStorageAccount(args);
        
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
        
        List<SecretArgs> secretListArgs = [];
        if (args is { EnableVault: true, Secrets.IsEmpty: false })
        {
            _secrets = args.Secrets.Select(secret => SecretFactory.Create(new CreateSecretArgs
                {
                    VaultName = (_vault?.Name ?? _existentVault?.Apply(vault => vault.Name))!,
                    Name = secret.Item1,
                    Alias = secret.Item2,
                    Value = secret.Item3,
                    Tags = args.Tags,
                    ResourceGroupName = args.ResourceGroupName,
                    Location = null,
                    Parent = this
                }))
                .ToImmutableList();

            secretListArgs = _secrets.Select(secret => new SecretArgs
            {
                Identity = _managedIdentity.Id,
                KeyVaultUrl = secret.Properties.Apply(props => props.SecretUri),
                Name = secret.Name
            }).ToList();
        }
        
        if (args is { EnableConfigStore: true,  AppConfig.IsEmpty: false })
        {
            args.AppConfig.Select(config => KeyValueFactory.Create(new CreateKeyValueArgs
            {
                ConfigStoreName = (_configurationStore?.Name ??
                                   _existentConfigStore?.Apply(configStore => configStore.Name))!,
                ResourceGroupName = args.ResourceGroupName,
                Name = config.Item1,
                Alias = config.Item2,
                Value = config.Item3,
                Tags = args.Tags,
                Parent = this
            })).ToList();
        }

        List<ContainerAppProbeArgs> probes = [];
        if (args.EnableProbes)
        {
            probes =
            [
                new ContainerAppProbeArgs
                {
                    FailureThreshold= 3,
                    HttpGet= new ContainerAppProbeHttpGetArgs
                    {
                        Port = args.Port,
                        Path = args.Path
                    },
                    InitialDelaySeconds = args.InitialDelaySeconds,
                    PeriodSeconds = args.PeriodSeconds,
                    SuccessThreshold = 1,
                    Type = Type.Liveness
                }
            ];
        }
        
        var containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            ContainerAppName = Output.Format($"{args.Name}-app-{args.Environment}"),
            EnvironmentId =  _managedEnvironment.Id,
            Identity = new ManagedServiceIdentityArgs
            {
                Type = ManagedServiceIdentityType.UserAssigned,
                UserAssignedIdentities = new[] { _managedIdentity.Id }
            },
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
                RevisionSuffix = Output.Format($"{args.ParentName}"),
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

    private void InitializeVault(DefaultAppComponentArgs args)
    {
        if (!args.EnableVault) return;
        
        if (!string.IsNullOrEmpty(args.VaultName))
        {
            _existentVault = OneBankHelper.GetKeyVault(args.ResourceGroupName, args.VaultName);
             return;
        }

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
        RoleAssignmentFactory.Create(new CreateRoleAssignmentArgs
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
    }
    
    private void InitializeConfigStore(DefaultAppComponentArgs args)
    {
        if (!args.EnableConfigStore) return;
        
        if (!string.IsNullOrEmpty(args.ConfigStoreName))
        {
            _existentConfigStore = OneBankHelper.GetConfigStore(args.ResourceGroupName, args.ConfigStoreName);
            return;
        }

        _configurationStore = ConfigurationStoreFactory.Create(new CreateConfigurationStoreArgs
        {
            Name = Output.Format($"{args.ParentName}-configStore-{args.Environment}"),
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            Parent = this,
            Tags = args.Tags,
        });
        
        var appConfigDataReaderRole = OneBankHelper.GetRoleDefinition(BuiltInRole.AppConfigurationDataReader, _configurationStore.Id).Apply(rd => rd.Id);
        RoleAssignmentFactory.Create(new CreateRoleAssignmentArgs
        {
            Name = new RandomUuid("OneBank_RoleAssignment_App_ConfigStore_UUID", new RandomUuidArgs { Keepers =
                {
                    { "ResourceGroupName", args.ResourceGroupName }, 
                    { "ManagedIdentityId", _managedIdentity.Id },
                    { "RoleDefinitionId", appConfigDataReaderRole }
                }
            }, new CustomResourceOptions { Parent = this }).Result,
            Alias = "App_ConfigStore",
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            RoleDefinitionId = appConfigDataReaderRole,
            PrincipalId = _managedIdentity.PrincipalId,
            Scope = _configurationStore.Id,
            Parent = this,
            Tags = args.Tags
        });
    }
    
    private void InitializeStorageAccount(DefaultAppComponentArgs args)
    {
        if (!args.EnableStorage) return;
        
        if (!string.IsNullOrEmpty(args.StorageAccountName))
        {
            _existentStorageAccount = OneBankHelper.GetStorage(args.ResourceGroupName, args.StorageAccountName);
            return;
        }

        _storageAccount = StorageAccountFactory.Create(new CreateStorageAccountArgs
        {
            Name = Output.Format($"{args.ParentName}sa{args.Environment}"),
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            Parent = this,
            EncryptionEnabled =  args.EnableEncryption,
            ImmutableStorageEnabled = false,
            Tags = args.Tags,
        });
        
        var storageBlobDataReaderRole = OneBankHelper.GetRoleDefinition(BuiltInRole.StorageBlobDataReader, _storageAccount.Id).Apply(rd => rd.Id);
        RoleAssignmentFactory.Create(new CreateRoleAssignmentArgs
        {
            Name = new RandomUuid("OneBank_RoleAssignment_App_StorageAccount_UUID", new RandomUuidArgs { Keepers =
                {
                    { "ResourceGroupName", args.ResourceGroupName }, 
                    { "ManagedIdentityId", _managedIdentity.Id },
                    { "RoleDefinitionId", storageBlobDataReaderRole }
                }
            }, new CustomResourceOptions { Parent = this }).Result,
            Alias = "App_StorageAccount",
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            RoleDefinitionId = storageBlobDataReaderRole,
            PrincipalId = _managedIdentity.PrincipalId,
            Scope = _storageAccount.Id,
            Parent = this,
            Tags = args.Tags
        });
    }
}