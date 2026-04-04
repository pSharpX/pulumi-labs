using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using defaultapp.components;
using Pulumi;
using Pulumi.AzureNative.Resources;
using OutputExtensions = csharp.Shared.OutputExtensions;

namespace defaultapp.Builders;

public class WebAppComponentBuilder: IComponentBuilder
{
    private ResourceGroup _resourceGroup;
    private WebAppComponent _componentResource;
    
    public InfrastructureResult Build(BuilderArgs args)
    {
        _resourceGroup = args.ResourceGroup;
        _componentResource = new WebAppComponent("OneBankWebAppComponent", BuildArgs(args.Config),
            new ComponentResourceOptions
            {
                DependsOn = args.DependsOn!
            });
        return new InfrastructureResult
        {
            Application = _componentResource,
            Output = OutputExtensions.CreateDictOutput(new Dictionary<string, Output<string>>
            {
                ["Endpoint"] = _componentResource.Endpoint,
            })
        };
    }
    
    private  WebAppComponentArgs BuildArgs(Config config)
    {
        var location = config.Get("location");
        var environment = config.Require("environment");
        var tags = config.RequireObject<Dictionary<string, string>>("tags");
        var resourceGroupName = config.Require("resourceGroupName");
        var applicationName = config.Require("applicationName");
        var applicationId = config.Require("applicationId");
        var imageName = config.Get("imageName");
        var repositoryUrl = config.Get("repositoryUrl");
        var branch = config.Get("branch");
        var runtime = config.Get("runtime");
        var startupCommandLine = config.Get("startupCommandLine");
        var port = config.GetInt32("port");
        var isPrivate = config.RequireBoolean("private");
        var vnetConfig = config.RequireObject<VirtualNetworkConfig>("vnetConfig");
        var databaseConfig = config.RequireObject<DatabaseConfig>("databaseConfig");
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
        var appSettings = config.GetObject<List< string[]>>("appSettings");
        var enableVault = config.RequireBoolean("enableVault");
        var keyVaultName = config.Get("vaultName");
        var enableConfigStore = config.RequireBoolean("enableConfigStore");
        var configStoreName = config.Get("configStoreName");
        var enableRegistry = config.RequireBoolean("enableRegistry");
        var registryName = config.Get("registryName");
        var enableEncryption = config.RequireBoolean("enableEncryption");
        var encryptionKeyName = config.Get("encryptionKeyName");
        var enableStorage = config.RequireBoolean("enableStorage");
        var storageAccountName = config.Get("storageAccountName");
        var enableDatabase = config.RequireBoolean("enableDatabase");
        var databaseEngine = config.Get("databaseEngine");

        var clientConfig = OneBankHelper.GetClientConfigAsync().Result;

        return new WebAppComponentArgs
        {
            ClientId = clientConfig.ClientId,
            ObjectId = clientConfig.ObjectId,
            TenantId = clientConfig.TenantId,
            SubscriptionId = clientConfig.SubscriptionId,
            ResourceGroupName = _resourceGroup.Name,
            Location = location!,
            Environment = environment,
            Private = isPrivate,
            External = isExternal,
            VirtualNetworkConfig = vnetConfig,
            ParentName = applicationId,
            Name = applicationName,
            Image = imageName,
            RepositoryUrl = repositoryUrl,
            Branch = branch!,
            Runtime = runtime!,
            StartupCommandLine = startupCommandLine!,
            /*
            Port = port,
            TotalCpu = totalCpu,
            TotalMemory = totalMemory,
            */
            EnableScaling = enableScaling,
            MinInstances = minInstances,
            MaxInstances = maxInstances,
            AllowedOrigins = allowedOrigins,
            /*
            AllowedMethods = allowedMethods,
            AllowedHeaders = allowedHeaders,
            EnableProbes = enableProbes,
            */
            Secrets = secrets?.Select(items => (items[0], items[1], items[2])).ToImmutableList()!,
            AppConfig = appConfig?.Select(items => (items[0], items[1], items[2])).ToImmutableList()!,
            AppSettings = appSettings?.Select(items => (items[0], items[1])).ToImmutableList()!,
            EnableVault = enableVault,
            VaultName = keyVaultName,
            EnableConfigStore = enableConfigStore,
            ConfigStoreName = configStoreName,
            EnableStorage = enableStorage,
            StorageAccountName = storageAccountName,
            EnableEncryption = enableEncryption,
            EncryptionKeyName = encryptionKeyName,
            EnableRegistry = enableRegistry,
            RegistryName = registryName,
            EnableDatabase = enableDatabase,
            DatabaseEngine = databaseEngine!,
            //DatabaseConfig = databaseConfig!,
            Tags = tags,
        };
    }
}