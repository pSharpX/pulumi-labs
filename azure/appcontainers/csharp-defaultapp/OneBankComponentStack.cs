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
            Image = imageName,
            ParentName = applicationId,
            Name = applicationName,
            Tags = tags
        });

        Endpoint = _componentResource.Endpoint;
    }
    
    [Output]
    public Output<string> Endpoint { get; private set; }
}