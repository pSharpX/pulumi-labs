using System;
using System.Collections.Generic;
using System.Linq;
using defaultapp.Shared;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace defaultapp.Factories;

public static class WebAppFactory
{
    public static WebApp Create(CreateWebAppArgs args)
    {
        var siteConfig = new SiteConfigArgs
        {
            PublicNetworkAccess = args.PublicNetworkAccess,
            AutoHealEnabled = true,
            Cors = new CorsSettingsArgs
            {
                AllowedOrigins = args.AllowedOrigins
            },
            Http20Enabled = true,
            MinTlsVersion = SupportedTlsVersions.SupportedTlsVersions_1_2,
        };

        if (args.Containerized)
        {
            if (args.ImageName is null) throw new ArgumentNullException(nameof(args.ImageName));

            if (args.RegistryName is null)
            {
                args.AppSettings.Add("DOCKER_REGISTRY_SERVICE_URL", "https://index.docker.io");
                args.AppSettings.Add("DOCKER_REGISTRY_SERVER_USERNAME", "");
                args.AppSettings.Add("DOCKER_REGISTRY_SERVER_PASSWORD", "");
            }
            
            if (args.IsLinux)
            {
                args.Kind = "app,linux,container";
                siteConfig.LinuxFxVersion = $"DOCKER|{args.ImageName}:{args.ImageTag}";
            }
            else
            {
                args.Kind = "app,container,windows";
                siteConfig.WindowsFxVersion = $"DOCKER|{args.ImageName}:{args.ImageTag}";
            }
        }
        else
        {
            if (args.Runtime is null) throw new ArgumentNullException(nameof(args.Runtime));
            
            if (args.IsLinux)
            {
                List<WebAppRuntime> runtimes = new WebAppRuntimeParser(args.Runtime).Runtimes;

                args.Kind = "app,linux";
                siteConfig.LinuxFxVersion = runtimes.First().ToString();
            }
            else
            {
                List<WebAppRuntime> runtimes = new WebAppWindowsRuntimeParser(args.Runtime).Runtimes;
                
                args.Kind = "app";
                foreach (var runtime in runtimes)
                {
                    switch (runtime.Name)
                    {
                        case "DOTNETCORE":
                            siteConfig.NetFrameworkVersion = runtime.Version;
                            break;
                        case "PHP":
                            siteConfig.PhpVersion = runtime.Version;
                            break;
                        case "PYTHON":
                            siteConfig.PythonVersion = runtime.Version;
                            break;
                        case "JAVA":
                            siteConfig.JavaVersion = runtime.Version;
                            break;
                        case "JAVACONTAINER":
                            siteConfig.JavaContainer = runtime.Version;
                            break;
                        case "JAVACONTAINERVERSION":
                            siteConfig.JavaContainerVersion = runtime.Version;
                            break;
                        case "NODE":
                            siteConfig.NodeVersion = runtime.Version;
                            break;
                    }
                }
            }
        }

        if (args.StartupCommandLine is not null)
        {
            siteConfig.AppCommandLine = args.StartupCommandLine;
        }
        if (args.HealthCheckPath is not null)
        {
            siteConfig.HealthCheckPath = args.HealthCheckPath;
        }

        siteConfig.AppSettings = args.AppSettings.Select(item => new NameValuePairArgs
        {
            Name = item.Key,
            Value = item.Value
        }).ToList();
        
        return new WebApp($"OneBank_WebApp_{args.Alias}", new WebAppArgs
        {
            Name = args.Name,
            Kind = args.Kind,
            ServerFarmId = args.ServicePlanId,
            Location = args.Location,
            ResourceGroupName =  args.ResourceGroupName,
            HttpsOnly = args.HttpsOnly,
            Enabled = args.Enabled,
            Identity = new ManagedServiceIdentityArgs
            {
                Type =  ManagedServiceIdentityType.UserAssigned,
                UserAssignedIdentities = args.ManagedIdentities
            },
            SiteConfig = siteConfig,
            PublicNetworkAccess = args.PublicNetworkAccess,
            Reserved = args.IsLinux,
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}