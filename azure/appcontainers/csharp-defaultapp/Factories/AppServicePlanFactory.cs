using defaultapp.Shared;
using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace defaultapp.Factories;

public static class AppServicePlanFactory
{
    public static AppServicePlan Create(CreateAppServicePlanArgs args)
    {
        args.ValidateServicePlan();
        
        var appServicePlanArgs = new AppServicePlanArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Name = args.Name,
            Kind = args.Kind,
            Sku = new SkuDescriptionArgs
            {
                Name = args.SkuName,
            },
            Reserved = args.Reserved,
            PerSiteScaling = true,
            Tags = args.Tags!,
        };

        if (args.Isolated)
        {
            appServicePlanArgs.HostingEnvironmentProfile = new HostingEnvironmentProfileArgs
            {
                Id = args.EnvironmentId
            };
        }

        if (args.EnableScaling)
        {
            appServicePlanArgs.ElasticScaleEnabled = true;
            appServicePlanArgs.Sku = new SkuDescriptionArgs
            {
                Name = args.SkuName,
                SkuCapacity = new SkuCapacityArgs
                {
                    Maximum = args.MaxInstances,
                    Minimum = args.MinInstances,
                }
            };
        }
        return new AppServicePlan("OneBank_AppServicePlan", appServicePlanArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}