using Pulumi;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;

namespace defaultapp.Factories;

public static class LogAnalyticsWorkspaceFactory
{
    public static Workspace Create(CreateLogAnalyticsWorkspaceArgs args)
    {
        return new Workspace("OneBank_OperationalInsightsWorkspace", new WorkspaceArgs
        {
            WorkspaceName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Sku = new WorkspaceSkuArgs()
            {
                Name = args.SkuName,
            },
            RetentionInDays = args.RetentionInDays,
            Tags = args.Tags!
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}