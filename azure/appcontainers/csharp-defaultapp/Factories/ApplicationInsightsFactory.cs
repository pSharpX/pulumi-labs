using Pulumi;
using Pulumi.AzureNative.ApplicationInsights;

namespace defaultapp.Factories;

public static class ApplicationInsightsFactory
{
    public static Component Create(CreateApplicationInsightsArgs args)
    {
        return new Component("OneBank_ApplicationInsight", new ComponentArgs
        {
            ResourceName =  args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            ApplicationType =  args.ApplicationType,
            Kind = args.Kind,
            Tags = args.Tags!,
            WorkspaceResourceId = args.WorkspaceId,
            DisableIpMasking = args.DisableIpMasking,
            RetentionInDays = args.RetentionInDays
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}