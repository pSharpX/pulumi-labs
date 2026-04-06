using System.Collections.Generic;
using System.Linq;
using defaultapp.Factories;
using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.ApplicationInsights;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.PrivateDns;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;

namespace defaultapp.components;

public class WebAppComponent: ComponentResource
{
    private readonly UserAssignedIdentity _managedIdentity;
    private readonly AppServicePlan _appServicePlan;
    private readonly WebApp _webApp;
    
    private PublicIPAddress? _publicIpAddress;
    private PrivateEndpoint? _privateEndpoint;
    private PrivateZone? _privateZone;
    private ApplicationGateway? _applicationGateway;
    private VirtualNetwork? _virtualNetwork;
    private Output<GetVirtualNetworkResult>? _existentVirtualNetwork;
    private List<Subnet>? _publicSubnets;
    private List<Subnet>? _privateSubnets;
    private Output<GetSubnetResult>? _existentSubnet;
    private Output<string>? _defaultPrivateSubnetId;
    private Output<string>? _defaultPrivateEndpointSubnetId;
    private Output<string>? _defaultPublicSubnetId;
    
    private readonly Workspace? _workspace;
    private readonly Component? _applicationInsights;
    private WebAppSourceControl? _sourceControl;
    private Vault? _vault;
    private ConfigurationStore? _configurationStore;
    private StorageAccount? _storageAccount;
    
    private Server? _sqlServer;
    private Database? _sqlDatabase;
    
    public Output<string> Endpoint { get; private set; }
    
    public WebAppComponent(string name, WebAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:WebAppComponent", name, options)
    {
        _managedIdentity = UserAssignedIdentityFactory.Create(new CreateUserAssignedIdentityArgs
        {
            Name = Output.Format($"{args.ParentName}-managed-identity-{args.Environment}"),
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            Parent = this,
            Tags = args.Tags
        });

        if (args.EnableInsights)
        {
            _workspace = LogAnalyticsWorkspaceFactory.Create(new CreateLogAnalyticsWorkspaceArgs
            {
                Name = Output.Format($"{args.ParentName}-cluster-logws-{args.Environment}"),
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                Tags = args.Tags,
                Parent = this
            });

            _applicationInsights = ApplicationInsightsFactory.Create(new CreateApplicationInsightsArgs
            {
                Name = Output.Format($"{args.ParentName}-appinsights-{args.Environment}"),
                WorkspaceId = _workspace.Id,
                ApplicationType = "python",
                Kind = "python",
                ResourceGroupName = args.ResourceGroupName,
                Location = args.Location,
                Tags = args.Tags,
                Parent = this,
            });
        }
        
        InitializeVirtualNetwork(args);

        _appServicePlan = AppServicePlanFactory.Create(new CreateAppServicePlanArgs
        {
            Name =  Output.Format($"{args.ParentName}-managed-plan-{args.Environment}"),
            SkuName = "B1",
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            Parent =  this,
            Tags = args.Tags
        });

        var webAppArgs = new CreateWebAppArgs
        {
            Name = Output.Format($"{args.ParentName}-{args.Name}-webapp-{args.Environment}"),
            Alias = "bookstore",
            Location = args.Location,
            ResourceGroupName = args.ResourceGroupName,
            ServicePlanId = _appServicePlan.Id,
            ManagedIdentities = [_managedIdentity.Id],
            Containerized = false,
            Runtime = args.Runtime,
            StartupCommandLine = args.StartupCommandLine,
            IsLinux = true,
            HealthCheckPath = args.HealthCheckPath,
            PublicNetworkAccess = args.Private ? "Disabled" : "Enabled",
            AllowedOrigins = args.AllowedOrigins,
            AppSettings = args.AppSettings.ToDictionary(item => item.Item1, item =>
            {
                Input<string> keyValue = item.Item2;
                return keyValue;
            }),
            HttpsOnly = true,
            Parent = this,
            Tags = args.Tags
        };

        if (!string.IsNullOrEmpty(args.Image))
        {
            webAppArgs.ImageName = args.Image;
            webAppArgs.ImageTag = args.ImageVersion;
            webAppArgs.Containerized = true;
            webAppArgs.Runtime = null;
            webAppArgs.StartupCommandLine = null;
        }

        if (args.EnableInsights)
        {
            webAppArgs.AppInsightsEnabled = args.EnableInsights;
            webAppArgs.Stack = args.Stack;
            webAppArgs.AppInsightsInstrumentationKey = _applicationInsights?.InstrumentationKey!;
            webAppArgs.AppInsightsConnectionString = _applicationInsights?.ConnectionString!;
        }
        
        _webApp = WebAppFactory.Create(webAppArgs);

        if (!string.IsNullOrEmpty(args.RepositoryUrl))
        {
            _sourceControl = WebAppSourceControlFactory.Create(new CreateWebAppSourceControlArgs
            {
                Alias = "bookstore",
                Name = _webApp.Name,
                RepositoryUrl =  args.RepositoryUrl,
                Branch =  args.Branch,
                ResourceGroupName = args.ResourceGroupName,
            });
        }

        if (args.Private)
        {
            WebAppSwiftVirtualNetworkConnectionFactory.Create(new CreateWebAppSwiftVirtualNetworkConnectionArgs
            {
                SubnetId = _defaultPrivateSubnetId!,
                Name = _webApp.Name,
                ResourceGroupName = args.ResourceGroupName,
                Parent = this,
            });
        }

        Endpoint = _webApp.DefaultHostName;
        
        InitializePrivateConnection(args);
        
        RegisterOutputs(new Dictionary<string, object?>
        {
            {"Endpoint", Endpoint},
        });
    }
    
    private void InitializeVirtualNetwork(WebAppComponentArgs args)
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
        
        Subnet serverFarmSubnet = args.VirtualNetworkConfig.Subnets
            .Where(subnet => subnet.Tags.Contains("private") && subnet.Alias.Equals("server-farms"))
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
            .Where(subnet => subnet.Tags.Contains("private") && !subnet.Alias.Equals("server-farms") && !subnet.Alias.Equals("private-endpoint"))
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

        _defaultPrivateSubnetId = serverFarmSubnet.Id;
        _defaultPublicSubnetId = gatewayIngressSubnet.Id;
        _defaultPrivateEndpointSubnetId = privateEndpointSubnet.Id;
    }
    
    private void InitializePrivateConnection(WebAppComponentArgs args)
    {
        if (!args.Private || args.VirtualNetworkConfig is null) return;

        var defaultDomain = "privatelink.azurewebsites.net";
        var staticIp = _webApp.DefaultHostName;

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
                PrivateLinkServiceId = _webApp.Id,
                GroupId = "sites",
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
                BackendFqdn = [Endpoint],//[Endpoint],
                PublicIpAddressId = _publicIpAddress.Id,
                BackendPort = 443,
                BackendProtocol =  "Https",
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