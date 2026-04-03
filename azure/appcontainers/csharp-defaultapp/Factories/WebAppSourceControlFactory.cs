using Pulumi;
using Pulumi.AzureNative.Web;

namespace defaultapp.Factories;

public static class WebAppSourceControlFactory
{
    public static WebAppSourceControl Create(CreateWebAppSourceControlArgs args)
    {
        return new WebAppSourceControl($"OneBank_WebAppSourceControl_{args.Alias}", new WebAppSourceControlArgs
        {
            Name = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Branch = args.Branch,
            RepoUrl = args.RepositoryUrl,
            IsManualIntegration = true,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}