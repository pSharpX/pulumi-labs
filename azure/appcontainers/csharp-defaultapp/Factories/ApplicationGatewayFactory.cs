using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;

namespace defaultapp.Factories;

public static class ApplicationGatewayFactory
{
    public static async Task<ApplicationGateway> Create(CreateApplicationGatewayArgs args)
    {
        List<ApplicationGatewayIPConfigurationArgs> gatewayIpConfigurations = [];
        List<ApplicationGatewayBackendAddressPoolArgs> backendAddressPools = [];
        List<ApplicationGatewayFrontendIPConfigurationArgs> frontendIpConfigurations = [];
        List<ApplicationGatewayFrontendPortArgs> frontendPorts = [];
        List<ApplicationGatewayHttpListenerArgs> httpListeners = [];
        List<ApplicationGatewayBackendHttpSettingsArgs> backendHttpSettingsCollection = [];
        List<ApplicationGatewayRequestRoutingRuleArgs> requestRoutingRules = [];
        
        if (await args.BackendFqdn.CountAsync() > 0)
        {
            args.SubnetId?.Ensure(value => !string.IsNullOrEmpty(value), $"{nameof(args.SubnetId)}");
            args.PublicIpAddressId?.Ensure(value => !string.IsNullOrEmpty(value), $"{nameof(args.PublicIpAddressId)}");
            
            Output<string> defaultBackendPool = Output.Format($"{args.Name}-backend-pool");
            Output<string> defaultGatewayIpConfig = Output.Format($"{args.Name}-alb-ipconfig");
            Output<string> defaultFrontendIpConfig = Output.Format($"{args.Name}-frontend-ipconfig");
            Output<string> defaultFrontendPorts = Output.Format($"{args.Name}-frontend-ports");
            Output<string> defaultHttpListener = Output.Format($"{args.Name}-listener");
            Output<string> defaultBackendHttpSettings = Output.Format($"{args.Name}-backend-settings");
            Output<string> defaultRoutingRules = Output.Format($"{args.Name}-routing-rules");
            
            backendAddressPools =
            [
                new ApplicationGatewayBackendAddressPoolArgs
                {
                    Name = defaultBackendPool,
                    BackendAddresses = await args.BackendFqdn.Select(fqdn => new ApplicationGatewayBackendAddressArgs
                    {
                        Fqdn = fqdn
                    }).ToListAsync()
                }
            ];

            gatewayIpConfigurations =
            [
                new ApplicationGatewayIPConfigurationArgs
                {
                    Name =  defaultGatewayIpConfig,
                    Subnet = new SubResourceArgs
                    {
                        Id =  args.SubnetId!,
                    }
                }
            ];

            frontendIpConfigurations = 
            [
                new ApplicationGatewayFrontendIPConfigurationArgs
                {
                    Name =  defaultFrontendIpConfig,
                    PublicIPAddress = new SubResourceArgs
                    {
                        Id = args.PublicIpAddressId!,
                    }
                }
            ];

            frontendPorts = 
            [
                new ApplicationGatewayFrontendPortArgs
                {
                    Name =  defaultFrontendPorts,
                    Port =  args.Port,
                }
            ];

            httpListeners = 
            [
                new ApplicationGatewayHttpListenerArgs
                {
                    Name = defaultHttpListener,
                    FrontendIPConfiguration = new SubResourceArgs
                    {
                        Id = BuildResourceId(args.SubscriptionId!, args.ResourceGroupName, args.Name, "frontendIPConfigurations", defaultFrontendIpConfig)
                    },
                    FrontendPort = new SubResourceArgs
                    {
                        Id = BuildResourceId(args.SubscriptionId!, args.ResourceGroupName, args.Name, "frontendPorts", defaultFrontendPorts)
                    },
                    Protocol = args.Protocol
                }    
            ];

            backendHttpSettingsCollection = 
            [
                new ApplicationGatewayBackendHttpSettingsArgs
                {
                    Name = defaultBackendHttpSettings,
                    Port = args.BackendPort,
                    Path = args.Path,
                    Protocol = args.BackendProtocol,
                    PickHostNameFromBackendAddress = true,
                    CookieBasedAffinity = "Disabled",
                    RequestTimeout = args.RequestTimeout
                }
            ];
            
            requestRoutingRules = 
            [
                new ApplicationGatewayRequestRoutingRuleArgs
                {
                    Name = defaultRoutingRules,
                    BackendAddressPool = new SubResourceArgs
                    {
                        Id  =  BuildResourceId(args.SubscriptionId!, args.ResourceGroupName, args.Name, "backendAddressPools", defaultBackendPool)
                    },
                    HttpListener =  new SubResourceArgs
                    {
                        Id = BuildResourceId(args.SubscriptionId!, args.ResourceGroupName, args.Name, "httpListeners", defaultHttpListener)
                    },
                    BackendHttpSettings =  new SubResourceArgs
                    {
                        Id = BuildResourceId(args.SubscriptionId!, args.ResourceGroupName, args.Name, "backendHttpSettingsCollection", defaultBackendHttpSettings)
                    },
                    RuleType =  args.RoutingRule,
                    Priority = args.Priority
                }
            ];
        }
        if (
            await args.GatewayIpConfigurations.CountAsync() > 0 &&
            await args.BackendAddressPools.CountAsync() > 0
            )
        {
            if (args.FrontendIPConfigurations.Count == 0) throw new ArgumentNullException($"{nameof(args.FrontendIPConfigurations)}");
            if (args.FrontendPorts.Count == 0) throw new ArgumentNullException($"{nameof(args.FrontendPorts)}");
            if (args.HttpListeners.Count == 0) throw new ArgumentNullException($"{nameof(args.HttpListeners)}");
            if (args.BackendHttpSettingsCollection.Count == 0) throw new ArgumentNullException($"{nameof(args.BackendHttpSettingsCollection)}");
            if (args.RequestRoutingRules.Count == 0) throw new ArgumentNullException($"{nameof(args.RequestRoutingRules)}");
            
            gatewayIpConfigurations = await args.GatewayIpConfigurations
                .Select(config => new ApplicationGatewayIPConfigurationArgs
                {
                    Name =  config.Apply(items => items.Item1),
                    Subnet = new SubResourceArgs
                    {
                        Id =  config.Apply(items => items.Item2)
                    }
                }).ToListAsync();
            
            backendAddressPools = await args.BackendAddressPools
                .Select(config => new ApplicationGatewayBackendAddressPoolArgs
                {
                    Name =   config.Apply(items => items.Item1),
                    BackendAddresses = config.Apply(items => items.Item2
                        .Select(fqdn => new ApplicationGatewayBackendAddressArgs
                        {
                            Fqdn = fqdn
                        }).ToList())
                }).ToListAsync();
            
            frontendIpConfigurations = args.FrontendIPConfigurations
                .Select(config =>
                    new ApplicationGatewayFrontendIPConfigurationArgs
                    {
                        Name =  config.Item1,
                        PublicIPAddress = new SubResourceArgs
                        {
                            Id = config.Item2,
                        }
                    }).ToList();
            
            frontendPorts = args.FrontendPorts
                .Select(config => new ApplicationGatewayFrontendPortArgs
                {
                    Name =  config.Item1,
                    Port =  config.Item2,
                }).ToList();
            
            httpListeners = args.HttpListeners
                .Select(config => new ApplicationGatewayHttpListenerArgs
                {
                    Name = config.Item1,
                    FrontendIPConfiguration = new SubResourceArgs
                    {
                        Id = config.Item2
                    },
                    FrontendPort = new SubResourceArgs
                    {
                        Id = config.Item3
                    },
                    Protocol = config.Item4
                }).ToList();
            
            backendHttpSettingsCollection = args.BackendHttpSettingsCollection
                .Select(config => new ApplicationGatewayBackendHttpSettingsArgs
                {
                    Name = config.Item1,
                    Port = config.Item2,
                    Path = config.Item3,
                    Protocol = args.BackendProtocol,
                    PickHostNameFromBackendAddress = config.Item4,
                    RequestTimeout = config.Item5
                }).ToList();
            
            requestRoutingRules = args.RequestRoutingRules
                .Select(config => new ApplicationGatewayRequestRoutingRuleArgs
                {
                    Name = config.Item1,
                    BackendAddressPool = new SubResourceArgs
                    {
                        Id  =  config.Item2,
                    },
                    HttpListener =  new SubResourceArgs
                    {
                        Id = config.Item3,
                    },
                    BackendHttpSettings =  new SubResourceArgs
                    {
                        Id = config.Item4,
                    },
                    RuleType =  config.Item5,
                    Priority = config.Item6
                }).ToList();
        }
        
        return new ApplicationGateway($"OneBank_ApplicationGateway", new ApplicationGatewayArgs
        {
            ApplicationGatewayName =  args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Sku = new ApplicationGatewaySkuArgs
            {
                Name =  args.SkuName,
                Tier =  args.SkuTier,
                Capacity =   args.SkuCapacity,
                Family =   args.SkuFamily,
            },
            EnableHttp2 = args.EnableHttp2,
            GatewayIPConfigurations = gatewayIpConfigurations,
            BackendAddressPools = backendAddressPools,
            FrontendIPConfigurations = frontendIpConfigurations,
            FrontendPorts = frontendPorts,
            HttpListeners = httpListeners,
            RequestRoutingRules = requestRoutingRules,
            BackendHttpSettingsCollection = backendHttpSettingsCollection,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }

    private static Output<string> BuildResourceId(Input<string> subscriptionId, Input<string> resourceGroupName, Input<string> applicationGateway, string parentName, Input<string> name) =>
        Output.Format(
            $"/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Network/applicationGateways/{applicationGateway}/{parentName}/{name}");
}