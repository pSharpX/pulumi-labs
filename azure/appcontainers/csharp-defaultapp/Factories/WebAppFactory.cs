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
        args.ValidateWebApp();
        
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
            WebSocketsEnabled = args.WebSocketsEnabled,
        };

        if (args.AppInsightsEnabled)
        {
            args.AppSettings.Add("APPINSIGHTS_INSTRUMENTATIONKEY", args.AppInsightsInstrumentationKey!);
            args.AppSettings.Add("APPLICATIONINSIGHTS_CONNECTION_STRING", args.AppInsightsConnectionString!);

            var agentExtensionVersion = args.IsLinux ? "~3" : "~2";
            switch (args.Stack)
            {
                case "python":
                    args.AppSettings.Add("ApplicationInsightsAgent_EXTENSION_VERSION", "~3");
                    break;
                case "java":
                    args.AppSettings.Add("ApplicationInsightsAgent_EXTENSION_VERSION", agentExtensionVersion);
                    if (!args.IsLinux)
                    {
                        args.AppSettings.Add("XDT_MicrosoftApplicationInsights_Java", "1");
                    }
                    break;
                case "Node.JS":
                    args.AppSettings.Add("ApplicationInsightsAgent_EXTENSION_VERSION", agentExtensionVersion);
                    if (!args.IsLinux)
                    {
                        args.AppSettings.Add("XDT_MicrosoftApplicationInsights_NodeJS", "1");
                    }
                    break;
                case "web":
                    args.AppSettings.Add("ApplicationInsightsAgent_EXTENSION_VERSION", agentExtensionVersion);
                    args.AppSettings.Add("XDT_MicrosoftApplicationInsights_Mode", "recommended");
                    args.AppSettings.Add("XDT_MicrosoftApplicationInsights_PreemptSdk", "1");
                    break;
            }
        }

        if (args.Containerized)
        {
            if (args.RegistryName is null)
            {
                args.AppSettings.Add("DOCKER_REGISTRY_SERVICE_URL", "https://index.docker.io");
                args.AppSettings.Add("DOCKER_REGISTRY_SERVER_USERNAME", "");
                args.AppSettings.Add("DOCKER_REGISTRY_SERVER_PASSWORD", "");
            }
            
            if (args.IsLinux)
            {
                args.Kind = "linux";
                siteConfig.LinuxFxVersion = $"DOCKER|{args.ImageName}:{args.ImageTag}";
            }
            else
            {
                args.Kind = "windows";
                siteConfig.WindowsFxVersion = $"DOCKER|{args.ImageName}:{args.ImageTag}";
            }
        }
        else
        {
            if (args.IsLinux)
            {
                List<WebAppRuntime> runtimes = new WebAppRuntimeParser(args.Runtime!).Runtimes;

                args.Kind = "app,linux";
                siteConfig.LinuxFxVersion = runtimes.First().ToString();
            }
            else
            {
                List<WebAppRuntime> runtimes = new WebAppWindowsRuntimeParser(args.Runtime!).Runtimes;
                
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