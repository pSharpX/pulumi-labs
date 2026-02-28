using System.Collections.Generic;
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
        var location = config.Require("location");
        var resourceGroupName = config.Require("resourceGroupName");
        var applicationName = config.Require("applicationName");
        var applicationId = config.Require("applicationId");
        var imageName = config.Require("imageName");
        var port = config.RequireInt32("port");
        var isPrivate = config.RequireBoolean("private");
        var isExternal = config.RequireBoolean("external");
        var totalCpu = config.RequireDouble("totalCpu");
        var totalMemory = config.Require("totalMemory");
        var enableScaling = config.RequireBoolean("enableScaling");
        var minInstances = config.RequireInt32("minInstances");
        var maxInstances =  config.RequireInt32("maxInstances");
        var tags = config.RequireObject<Dictionary<string, string>>("tags");   
        
        _resourceGroup = new ResourceGroup("TeamLvX_rg", new ResourceGroupArgs
        {
            ResourceGroupName = resourceGroupName,
            Tags = tags
        });

        _componentResource = new DefaultAppComponent("OneBankDefaultAppComponent", new DefaultAppComponentArgs
        {
            ResourceGroupName = _resourceGroup.Name,
            Location = location,
            Private = isPrivate,
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
            Tags = tags,
        });

        Endpoint = _componentResource.Endpoint;
    }
    
    [Output]
    public Output<string> Endpoint { get; private set; }
}