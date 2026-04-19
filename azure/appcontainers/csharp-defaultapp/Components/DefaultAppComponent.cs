using System;
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
using Pulumi.AzureNative.PrivateDns;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Storage;
using Pulumi.Random;
using ManagedServiceIdentityArgs = Pulumi.AzureNative.App.Inputs.ManagedServiceIdentityArgs;
using ManagedServiceIdentityType = Pulumi.AzureNative.App.ManagedServiceIdentityType;
using SecretArgs = Pulumi.AzureNative.App.Inputs.SecretArgs;
using Type = Pulumi.AzureNative.App.Type;

namespace defaultapp.components;

public class DefaultAppComponent: ComponentResource
{
    private readonly UserAssignedIdentity _managedIdentity;
    private readonly Workspace _workspace;
    private readonly ManagedEnvironment _managedEnvironment;
    private readonly ContainerApp _containerApp;

    private PublicIPAddress? _publicIpAddress;
    private PrivateEndpoint? _privateEndpoint;
    private PrivateZone? _privateZone;
    private ApplicationGateway? _applicationGateway;
    private NetworkSecurityGroup? _networkSecurityGroup;
    private VirtualNetwork? _virtualNetwork;
    private Output<GetVirtualNetworkResult>? _existentVirtualNetwork;
    private List<Subnet>? _publicSubnets;
    private List<Subnet>? _privateSubnets;
    private Output<GetSubnetResult>? _existentSubnet;
    private Output<string>? _defaultPrivateSubnetId;
    private Output<string>? _defaultPrivateEndpointSubnetId;
    private Output<string>? _defaultPublicSubnetId;
    
    private Vault? _vault;
    private Output<GetVaultResult>? _existentVault;
    
    private ConfigurationStore? _configurationStore;
    private Output<GetConfigurationStoreResult>? _existentConfigStore;

    private StorageAccount? _storageAccount;
    private Output<GetStorageAccountResult>? _existentStorageAccount;
    
    private Server? _sqlServer;
    private Database? _sqlDatabase;
    
    private ImmutableList<Secret>? _secrets;
    
    public Output<string> Endpoint { get; private set; }
    public Output<string>? DatabaseUrl { get; private set; }
    public Output<string>? StorageAccountUrl { get; private set; }
    public Output<string>? ConfigStoreEndpoint { get; private set; }
    public Output<string>? VaultUri { get; private set; }

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
        
        InitializeVirtualNetwork(args);
        InitializeVault(args);
        InitializeConfigStore(args);
        InitializeStorageAccount(args);
        InitializeDatabase(args);
        
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
            PublicNetworkAccess = args.Private ? "Disabled": "Enabled",
            VnetConfiguration = args.Private ? 
                new VnetConfigurationArgs
                {
                    Internal = args.Private,
                    InfrastructureSubnetId = _defaultPrivateSubnetId!
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
                        Path = args.HealthCheckPath
                    },
                    InitialDelaySeconds = args.InitialDelaySeconds,
                    PeriodSeconds = args.PeriodSeconds,
                    SuccessThreshold = 1,
                    Type = Type.Liveness
                }
            ];
        }
        
        _containerApp = new ContainerApp("OneBank_ContainerApp", new ContainerAppArgs
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
                    AllowInsecure = true,
                    External =  args.External,
                    TargetPort = args.Port,
                    Transport = IngressTransportMethod.Http,
                    CorsPolicy = new CorsPolicyArgs
                    {
                        AllowedOrigins = args.AllowedOrigins,
                        AllowedHeaders = args.AllowedHeaders,
                        AllowedMethods = args.AllowedMethods
                    },
                    Traffic = [
                        new TrafficWeightArgs
                        {
                            LatestRevision = true,
                            Weight = 100
                        }
                    ]
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
                Scale = args.EnableScaling ? new ScaleArgs
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
                }: null!
            },
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = this });

        Endpoint = _containerApp.Configuration.Apply(configuration => configuration?.Ingress?.Fqdn!);

        InitializePrivateConnection(args);

        RegisterOutputs(new Dictionary<string, object?>
        {
            {"Endpoint", Endpoint},
            {"DatabaseUrl", DatabaseUrl},
            {"StorageAccountUrl", StorageAccountUrl},
            {"VaultUri", VaultUri},
            {"ConfigStoreEndpoint", ConfigStoreEndpoint},
        });
    }

    private void InitializeVault(DefaultAppComponentArgs args)
    {
        if (!args.EnableVault) return;
        
        if (!string.IsNullOrEmpty(args.VaultName))
        {
            _existentVault = OneBankHelper.GetKeyVault(args.ResourceGroupName, args.VaultName);
            VaultUri = _existentVault.Apply(props => props.Properties.VaultUri);
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

        VaultUri = _vault.Properties.Apply(props => props.VaultUri);
    }
    
    private void InitializeConfigStore(DefaultAppComponentArgs args)
    {
        if (!args.EnableConfigStore) return;
        
        if (!string.IsNullOrEmpty(args.ConfigStoreName))
        {
            _existentConfigStore = OneBankHelper.GetConfigStore(args.ResourceGroupName, args.ConfigStoreName);
            ConfigStoreEndpoint = _existentConfigStore.Apply(props => props.Endpoint);
            return;
        }

        _configurationStore = ConfigurationStoreFactory.Create(new CreateConfigurationStoreArgs
        {
            Name = Output.Format($"{args.ParentName}-config-store-{args.Environment}"),
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            DisableLocalAuth = false,
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

        ConfigStoreEndpoint = _configurationStore.Endpoint;
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

        StorageAccountUrl = _storageAccount.PrimaryEndpoints.Apply(props => props.Blob);
    }

    private void InitializeDatabase(DefaultAppComponentArgs args)
    {
        if (!args.EnableDatabase) return;
        
        if (args.DatabaseConfig is null)
            throw new ArgumentNullException(nameof(args.DatabaseConfig));

        _sqlServer = SqlServerFactory.Create(new CreateSqlServerArgs
        {
            Name = Output.Format($"{args.ParentName}-server-{args.Environment}"),
            Alias = args.DatabaseConfig.DatabaseName!,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AdministratorLogin = args.DatabaseConfig.Username!,
            AdministratorLoginPassword = args.DatabaseConfig.Password!,
            Parent = this,
            Tags = args.Tags,
        });

        _sqlDatabase = SqlDatabaseFactory.Create(new CreateSqlDatabaseArgs
        {
            Name = Output.Format($"{args.Name}-database-{args.Environment}"),
            ServerName = _sqlServer.Name,
            Alias = args.DatabaseConfig.DatabaseName!,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Parent = this,
            Tags = args.Tags,
        });

        DatabaseUrl = _sqlServer.FullyQualifiedDomainName;
    }

    private void InitializeVirtualNetwork(DefaultAppComponentArgs args)
    {
        if (!args.Private || args.VirtualNetworkConfig is null) return;

        if (!string.IsNullOrEmpty(args.VirtualNetworkConfig.Name) &&  
            !string.IsNullOrEmpty(args.VirtualNetworkConfig.SubnetId) )
        {
            _existentVirtualNetwork = OneBankHelper.GetVirtualNetwork(args.ResourceGroupName, args.VirtualNetworkConfig?.Name!);
            _existentSubnet = OneBankHelper.GetSubnet(args.ResourceGroupName, args.VirtualNetworkConfig?.Name!, args.VirtualNetworkConfig?.SubnetId!);
            _defaultPrivateSubnetId = _existentSubnet.Apply(subnet => subnet.Id)!;
            return;    
        }

        _virtualNetwork = VirtualNetworkFactory.Create(new CreateVirtualNetworkArgs
        {
            Name = Output.Format($"{args.ParentName}-vnet-{args.Environment}"),
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location!,
            AddressPrefixes = args.VirtualNetworkConfig.AddressPrefixes!,
            Tags = args.Tags,
            Parent = this
        });
        
        Subnet gatewayIngressSubnet = args.VirtualNetworkConfig.Subnets
            .Where(subnet => !subnet.Tags.Contains("private") && subnet.Alias.Equals("gateway-ingress"))
            .Select(subnet => SubnetFactory.Create(new CreateSubnetArgs
            {
                Alias = subnet.Alias,
                VirtualNetworkName = _virtualNetwork.Name,
                Name = Output.Format($"{args.ParentName}-{subnet.Name}-subnet"),
                SubnetName = subnet.Name,
                SubnetAddressPrefixes= subnet.AddressPrefixes!,
                ResourceGroupName = args.ResourceGroupName,
                Delegations = subnet.Delegations.Select(delegation => (delegation.Name, delegation.ServiceName)).ToList(),
                Location = args.Location,
                Parent = this,
                Tags = args.Tags
            })).Single();
        
        Subnet privateEndpointSubnet = args.VirtualNetworkConfig.Subnets
            .Where(subnet => subnet.Tags.Contains("private") && subnet.Alias.Equals("private-endpoint"))
            .Select(subnet => SubnetFactory.Create(new CreateSubnetArgs
            {
                Alias = subnet.Alias,
                VirtualNetworkName = _virtualNetwork.Name,
                Name = Output.Format($"{args.ParentName}-{subnet.Name}-subnet"),
                SubnetName = subnet.Name,
                SubnetAddressPrefixes= subnet.AddressPrefixes!,
                ResourceGroupName = args.ResourceGroupName,
                Delegations = subnet.Delegations.Select(delegation => (delegation.Name, delegation.ServiceName)).ToList(),
                Location = args.Location,
                Parent = this,
                Tags = args.Tags
            })).Single();
        
        Subnet clusterSubnet = args.VirtualNetworkConfig.Subnets
            .Where(subnet => subnet.Tags.Contains("private") && subnet.Alias.Equals("cluster"))
            .Select(subnet => SubnetFactory.Create(new CreateSubnetArgs
            {
                Alias = subnet.Alias,
                VirtualNetworkName = _virtualNetwork.Name,
                Name = Output.Format($"{args.ParentName}-{subnet.Name}-subnet"),
                SubnetName = subnet.Name,
                SubnetAddressPrefixes= subnet.AddressPrefixes!,
                ResourceGroupName = args.ResourceGroupName,
                Delegations = subnet.Delegations.Select(delegation => (delegation.Name, delegation.ServiceName)).ToList(),
                Location = args.Location,
                Parent = this,
                Tags = args.Tags
            })).Single();

        _publicSubnets = args.VirtualNetworkConfig.Subnets
            .Where(subnet => !subnet.Tags.Contains("private") && !subnet.Alias.Equals("gateway-ingress"))
            .Select(subnet => SubnetFactory.Create(new CreateSubnetArgs
        {
            Alias = subnet.Alias,
            VirtualNetworkName = _virtualNetwork.Name,
            Name = Output.Format($"{args.ParentName}-{subnet.Name}-subnet"),
            SubnetName = subnet.Name,
            SubnetAddressPrefixes= subnet.AddressPrefixes!,
            ResourceGroupName = args.ResourceGroupName,
            Delegations = subnet.Delegations.Select(delegation => (delegation.Name, delegation.ServiceName)).ToList(),
            Location = args.Location,
            Parent = this,
            Tags = args.Tags
        })).ToList();

        _privateSubnets = args.VirtualNetworkConfig.Subnets
            .Where(subnet => subnet.Tags.Contains("private") && !subnet.Alias.Equals("cluster") && !subnet.Alias.Equals("private-endpoint"))
            .Select(subnet => SubnetFactory.Create(new CreateSubnetArgs
            {
                Alias = subnet.Alias,
                VirtualNetworkName = _virtualNetwork.Name,
                Name = Output.Format($"{args.ParentName}-{subnet.Name}-subnet"),
                SubnetName = subnet.Name,
                SubnetAddressPrefixes= subnet.AddressPrefixes!,
                ResourceGroupName = args.ResourceGroupName,
                Delegations = subnet.Delegations.Select(delegation => (delegation.Name, delegation.ServiceName)).ToList(),
                Location = args.Location,
                Parent = this,
                Tags = args.Tags
            })).ToList();

        _defaultPrivateSubnetId = clusterSubnet.Id;
        _defaultPublicSubnetId = gatewayIngressSubnet.Id;
        _defaultPrivateEndpointSubnetId = privateEndpointSubnet.Id;
    }

    private void InitializeSecurityGroups(DefaultAppComponentArgs args)
    {
        if (!args.Private || args.VirtualNetworkConfig is null) return;

        if (!string.IsNullOrEmpty(args.VirtualNetworkConfig.Name) &&
            !string.IsNullOrEmpty(args.VirtualNetworkConfig.SubnetId)) return;
        
        _networkSecurityGroup = NetworkSecurityGroupFactory.Create(new CreateNetworkSecurityGroupArgs
        {
            Name = Output.Format($"{args.ParentName}-{args.Name}-nsg-{args.Environment}"),
            ResourceGroupName = args.ResourceGroupName,
            Alias = "Bookstore",
            SecurityRules = [
                new CreateSecurityRuleArgs
                {
                    ResourceGroupName = args.ResourceGroupName,
                    Name = "allow-agw-traffic",
                    Alias = "allow-agw-traffic",
                    Description = "allow http inbound traffic from gateway-ingress subnet to private subnet",
                    Access = "Allow",
                    Direction = "Inbound",
                    DestinationAddressPrefixes = ["10.1.1.0/24"],
                    DestinationPortRange = "*",
                    Priority = 100,
                    Protocol = "Tcp",
                    SourceAddressPrefixes = [ "10.1.10.0/24" , "10.1.11.0/24"],
                    SourcePortRange = "*",
                }
            ],
            Location = args.Location,
            Tags = args.Tags,
            Parent = this
        });
    }

    private void InitializePrivateConnection(DefaultAppComponentArgs args)
    {
        if (!args.Private || args.VirtualNetworkConfig is null) return;

        var defaultDomain = _managedEnvironment.DefaultDomain;
        var staticIp = _managedEnvironment.StaticIp;

        _privateZone = PrivateDnsFactory.Create(new CreateDnsZoneArgs
        {
            Alias = "bookstore",
            Name = defaultDomain,
            ResourceGroupName = args.ResourceGroupName,
            Parent = this,
            Tags = args.Tags,
        });

        if (args.External)
        {
            _publicIpAddress = PublicIpAddressFactory.Create(new CreatePublicIpAddressArgs
            {
                Name = Output.Format($"{args.ParentName}-public-ip-{args.Environment}"),
                Alias = "bookstore",
                DnsNameLabel = args.ParentName,
                SkuName = "Standard",
                IpAllocationMethod = "Static",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                Parent = this,
                Tags = args.Tags,
            });
            
            _privateEndpoint = PrivateEndpointFactory.Create(new CreatePrivateEndpointArgs
            {
                Name = Output.Format($"{args.ParentName}-private-endpoint-{args.Environment}"),
                Alias = "bookstore",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                SubnetId = _defaultPrivateEndpointSubnetId!,
                PrivateLinkServiceId = _managedEnvironment.Id,
                GroupId = "managedEnvironments",
                Parent = this,
                Tags = args.Tags,
            });
            
            staticIp = _privateEndpoint.CustomDnsConfigs.Apply(dnsConfig => dnsConfig.First().IpAddresses.First());
            
            var applicationGatewayArgs = new CreateApplicationGatewayArgs
            {
                Name = Output.Format($"{args.ParentName}-alb-{args.Environment}"),
                SkuName = "Standard_v2",
                SkuTier = "Standard_v2",
                SkuFamily = "Generation_1",
                ResourceGroupName = args.ResourceGroupName,
                SubscriptionId = args.SubscriptionId,
                Location = args.Location,
                BackendFqdn = [Endpoint],
                PublicIpAddressId = _publicIpAddress.Id,
                SubnetId = Output.Tuple(_defaultPublicSubnetId!, _defaultPrivateSubnetId!)
                    .Apply(items => items.Item1 ?? items.Item2),
                Parent = this,
                Tags = args.Tags,
            };

            _applicationGateway = ApplicationGatewayFactory.Create(applicationGatewayArgs).Result;
            
            Endpoint = Output.Format($"http://{_publicIpAddress.DnsSettings.Apply(dns => dns?.Fqdn)}");
        }
        
        var starRecordSet = PrivateRecordSetFactory.Create(new CreateRecordSetArgs
        {
            Alias = "star",
            Name = "*",
            RecordType = "A",
            ZoneName = _privateZone.Name,
            Ipv4Address = staticIp,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Parent = this,
            Tags = args.Tags,
        });
        var atRecordSet = PrivateRecordSetFactory.Create(new CreateRecordSetArgs
        {
            Alias = "at",
            Name = "@",
            RecordType = "A",
            ZoneName = _privateZone.Name,
            Ipv4Address = staticIp,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Parent = this,
            Tags = args.Tags,
        });

        var virtualNetworkLink = VirtualNetworkLinkFactory.Create(new CreateVirtualNetworkLinkArgs
        {
            Alias = "bookstore",
            Name = Output.Format($"{args.ParentName}-vnet-pdns-link-{args.Environment}"),
            VirtualNetworkId = _virtualNetwork?.Id!,
            PrivateZoneName = _privateZone.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Parent = this,
            Tags = args.Tags!,
        });
    }
}