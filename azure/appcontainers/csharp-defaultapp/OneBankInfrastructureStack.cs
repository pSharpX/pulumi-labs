using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Pulumi;
using Pulumi.AzureNative.Resources;

namespace defaultapp;

public class OneBankInfrastructureStack: Stack
{
    private readonly ResourceGroup _resourceGroup;

    public OneBankInfrastructureStack()
    {
        IServiceCollection services = new ServiceCollection();

        Startup startup = new Startup();
        startup.ConfigureServices(services);
    
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        var infrastructureResolver = serviceProvider.GetRequiredService<IInfrastructureResolver>();
        
        var config = new Config("csharp-defaultapp");
        var componentType = Enum.TryParse<AppComponentType>(config.Get("componentType"), true, out var type) ? type: AppComponentType.DefaultApp;
        var tags = config.RequireObject<Dictionary<string, string>>("tags");
        var resourceGroupName = config.Require("resourceGroupName");
        var location = config.Require("location");
        
        _resourceGroup = new ResourceGroup("TeamLvX_rg", new ResourceGroupArgs
        {
            ResourceGroupName = resourceGroupName,
            Location = location,
            Tags = tags
        });
        InfrastructureResult result = infrastructureResolver.Resolve(componentType).Build(new BuilderArgs
        {
            Config = config,
            DependsOn = [_resourceGroup],
            ResourceGroup = _resourceGroup,
        });
        
        Output = result.Output;
    }
    
    [Output] public Output<Dictionary<string, string>> Output { get; private set; }
}