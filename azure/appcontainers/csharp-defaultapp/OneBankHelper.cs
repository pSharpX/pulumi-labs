using System.Threading.Tasks;
using Pulumi.AzureNative.Authorization;

namespace defaultapp;

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

    public static Output<GetClientConfigResult> GetClientConfig()
    {
        return Pulumi.AzureNative.Authorization.GetClientConfig.Invoke();
    }
    
    public static Task<GetClientConfigResult> GetClientConfigAsync()
    {
        return Pulumi.AzureNative.Authorization.GetClientConfig.InvokeAsync();
    }

    public static Output<GetRoleDefinitionResult> GetRoleDefinition(string roleDefinitionId, Input<string>scope)
    {
        return Pulumi.AzureNative.Authorization.GetRoleDefinition.Invoke(new GetRoleDefinitionInvokeArgs
        {
            RoleDefinitionId = roleDefinitionId,
            Scope = scope
        });
    }
}