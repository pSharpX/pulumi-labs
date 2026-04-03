using Pulumi;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace defaultapp.Factories;

public static class AppServicePlanFactory
{
    public static AppServicePlan Create(CreateAppServicePlanArgs args)
    {
        return new AppServicePlan("OneBank_AppServicePlan", new AppServicePlanArgs
        {
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Name = args.Name,
            Kind = args.Kind,
            Sku = new SkuDescriptionArgs
            {
                Name = args.SkuName,
            },
            Reserved = args.Kind.Contains("linux"),
            Tags = args.Tags!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}