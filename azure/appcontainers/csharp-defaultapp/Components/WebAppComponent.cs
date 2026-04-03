using System.Collections.Generic;
using System.Linq;
using defaultapp.Factories;
using Pulumi;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;

namespace defaultapp.components;

public class WebAppComponent: ComponentResource
{
    private readonly UserAssignedIdentity _managedIdentity;
    private readonly AppServicePlan _appServicePlan;
    private readonly WebApp _webApp;
    
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
            AppSettings = args.AppSettings.ToDictionary(item => item.Item1, item => item.Item2),
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

        Endpoint = Output.Format($"https://{_webApp.DefaultHostName}");
        
        RegisterOutputs(new Dictionary<string, object?>
        {
            {"Endpoint", Endpoint},
        });
    }
}