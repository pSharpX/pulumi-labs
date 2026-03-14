using System.Collections.Generic;
using System.Linq;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;

namespace defaultapp.Factories;

public static class ApplicationGatewayFactory
{
    public static ApplicationGateway Create(CreateApplicationGatewayArgs args)
    {
        List<ApplicationGatewayIPConfigurationArgs> gatewayIpConfigurations = [];
        if (args.GatewayIpConfigurations.Count > 0)
        {
            gatewayIpConfigurations = args.GatewayIpConfigurations
                .Select(config => new ApplicationGatewayIPConfigurationArgs
                {
                    Name =  config.Item1,
                    Subnet = new SubResourceArgs
                    {
                        Id =  config.Item2,
                    }
                }).ToList();
        }
        
        List<ApplicationGatewayBackendAddressPoolArgs> backendAddressPools = [];
        if (args.BackendAddressPools.Count > 0)
        {
            backendAddressPools = args.BackendAddressPools.Select(config => new ApplicationGatewayBackendAddressPoolArgs
            {
                Name =   config.Item1,
                BackendAddresses = config.Item2.Select(fqdn => new ApplicationGatewayBackendAddressArgs
                {
                    Fqdn = fqdn
                }).ToList()
            }).ToList();
        }

        List<ApplicationGatewayFrontendIPConfigurationArgs> frontendIpConfigurations = [];
        if (args.FrontendIPConfigurations.Count > 0)
        {
            frontendIpConfigurations = args.FrontendIPConfigurations.Select(config =>
                new ApplicationGatewayFrontendIPConfigurationArgs
                {
                    Name =  config.Item1,
                    PublicIPAddress = new SubResourceArgs
                    {
                        Id = config.Item2,
                    }
                }).ToList();
        }
        
        List<ApplicationGatewayFrontendPortArgs> frontendPorts = [];
        if (args.FrontendPorts.Count > 0)
        {
            frontendPorts = args.FrontendPorts.Select(config => new ApplicationGatewayFrontendPortArgs
            {
                Name =  config.Item1,
                Port =  config.Item2,
            }).ToList();
        }
        
        List<ApplicationGatewayHttpListenerArgs> httpListeners = [];
        if (args.HttpListeners.Count > 0)
        {
            httpListeners = args.HttpListeners.Select(config => new ApplicationGatewayHttpListenerArgs
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
        }
        
        List<ApplicationGatewayBackendHttpSettingsArgs> backendHttpSettingsCollection = [];
        if (args.BackendHttpSettingsCollection.Count > 0)
        {
            backendHttpSettingsCollection = args.BackendHttpSettingsCollection
                .Select(config => new ApplicationGatewayBackendHttpSettingsArgs
                {
                    Name = config.Item1,
                    Port = config.Item2,
                    Path = config.Item3,
                    PickHostNameFromBackendAddress = config.Item4,
                    RequestTimeout = config.Item5
                }).ToList();
        }
        
        List<ApplicationGatewayRequestRoutingRuleArgs> requestRoutingRules = [];
        if (args.RequestRoutingRules.Count > 0)
        {
            requestRoutingRules = args.RequestRoutingRules.Select(config => new ApplicationGatewayRequestRoutingRuleArgs
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
        
        return new ApplicationGateway("OneBank_ApplicationGateway_", new ApplicationGatewayArgs
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
}