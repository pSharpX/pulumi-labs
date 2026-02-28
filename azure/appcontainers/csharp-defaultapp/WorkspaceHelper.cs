using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.OperationalInsights;

public static class OneBankHelper
{
    public static Output<GetSharedKeysResult> GetWorkspaceSharedKeys(Output<string> resourceGroupName, Output<string> workspaceName)
    {
        return Output.Tuple(resourceGroupName, workspaceName)
            .Apply(items => GetSharedKeys.InvokeAsync(new GetSharedKeysArgs
            {
                ResourceGroupName = items.Item1,
                WorkspaceName = items.Item2
            }));
    }
}