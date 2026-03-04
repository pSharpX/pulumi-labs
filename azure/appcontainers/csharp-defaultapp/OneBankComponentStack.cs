using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using defaultapp.components;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace defaultapp;

public class OneBankComponentStack: Stack
{
    private readonly ResourceGroup _resourceGroup;
    private readonly DefaultAppComponent _componentResource;

    public OneBankComponentStack()
    {
        var config = new Config();
        var location = config.Get("location");
        var environment = config.Require("environment");
        var resourceGroupName = config.Require("resourceGroupName");
        var applicationName = config.Require("applicationName");
        var applicationId = config.Require("applicationId");
        var imageName = config.Require("imageName");
        var port = config.RequireInt32("port");
        var isPrivate = config.RequireBoolean("private");
        var addressPrefixes = config.GetObject<List<string>>("addressPrefixes");
        var subnetAddressPrefixes = config.GetObject<List<string>>("subnetAddressPrefixes");
        var isExternal = config.RequireBoolean("external");
        var totalCpu = config.RequireDouble("totalCpu");
        var totalMemory = config.Require("totalMemory");
        var enableScaling = config.RequireBoolean("enableScaling");
        var minInstances = config.RequireInt32("minInstances");
        var maxInstances =  config.RequireInt32("maxInstances");
        var allowedOrigins = config.RequireObject<List<string>>("allowedOrigins");
        var allowedMethods = config.RequireObject<List<string>>("allowedMethods");
        var allowedHeaders = config.RequireObject<List<string>>("allowedHeaders");
        var enableProbes = config.RequireBoolean("enableProbes");
        var secrets = config.GetObject<List< string[]>>("secrets");
        var appConfig = config.GetObject<List< string[]>>("appConfig");
        var tags = config.RequireObject<Dictionary<string, string>>("tags");

        var clientConfig = OneBankHelper.GetClientConfigAsync().Result;
        _resourceGroup = new ResourceGroup("TeamLvX_rg", new ResourceGroupArgs
        {
            ResourceGroupName = resourceGroupName,
            Tags = tags
        });
        _componentResource = new DefaultAppComponent("OneBankDefaultAppComponent", new DefaultAppComponentArgs
        {
            ClientId = clientConfig.ClientId,
            ObjectId = clientConfig.ObjectId,
            TenantId = clientConfig.TenantId,
            SubscriptionId = clientConfig.SubscriptionId,
            ResourceGroupName = _resourceGroup.Name,
            Location = location,
            Environment = environment,
            Private = isPrivate,
            AddressPrefixes = addressPrefixes!,
            SubnetAddressPrefixes = subnetAddressPrefixes!,
            Image = imageName,
            ParentName = applicationId,
            Name = applicationName,
            Port = port,
            TotalCpu = totalCpu,
            TotalMemory = totalMemory,
            External = isExternal,
            EnableScaling = enableScaling,
            MinInstances = minInstances,
            MaxInstances = maxInstances,
            AllowedOrigins = allowedOrigins,
            AllowedMethods = allowedMethods,
            AllowedHeaders = allowedHeaders,
            EnableProbes = enableProbes,
            Secrets = secrets?.Select(items => (items[0], items[1], items[2])).ToImmutableList()!,
            Tags = tags,
        });

        Endpoint = _componentResource.Endpoint;
    }
    
    [Output]
    public Output<string> Endpoint { get; private set; }
}