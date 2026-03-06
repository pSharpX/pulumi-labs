using System.Threading.Tasks;
using Pulumi.AzureNative.AppConfiguration;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.ContainerRegistry;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Storage;

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
    
    public static Output<GetRoleDefinitionResult> GetRoleDefinition(BuiltInRole builtInRole, Input<string>scope)
    {
        return Pulumi.AzureNative.Authorization.GetRoleDefinition.Invoke(new GetRoleDefinitionInvokeArgs
        {
            RoleDefinitionId = BuiltInRoleIds.Get(builtInRole),
            Scope = scope
        });
    }
    
    public static Output<GetConfigurationStoreResult> GetConfigStore(Input<string> resourceGroupName, Input<string> configStoreName)
    {
        return GetConfigurationStore.Invoke(new GetConfigurationStoreInvokeArgs
        {
            ResourceGroupName = resourceGroupName,
            ConfigStoreName = configStoreName
        });
    }
    
    public static Output<GetRegistryResult> GetContainerRegistry(Input<string> resourceGroupName, Input<string> registryName)
    {
        return GetRegistry.Invoke(new GetRegistryInvokeArgs
        {
            ResourceGroupName = resourceGroupName, 
            RegistryName = registryName,
        });
    }
    
    public static Output<GetStorageAccountResult> GetStorage(Input<string> resourceGroupName, Input<string> accountName)
    {
        return GetStorageAccount.Invoke(new GetStorageAccountInvokeArgs()
        {
            ResourceGroupName = resourceGroupName, 
            AccountName = accountName
        });
    }
    
    public static Output<GetVaultResult> GetKeyVault(Input<string> resourceGroupName, Input<string> vaultName)
    {
        return GetVault.Invoke(new GetVaultInvokeArgs
        {
            ResourceGroupName = resourceGroupName,
            VaultName = vaultName
        });
    }
    
    public static Output<GetVirtualNetworkResult> GetVNet(Input<string> resourceGroupName, Input<string> virtualNetworkName)
    {
        return GetVirtualNetwork.Invoke(new GetVirtualNetworkInvokeArgs
        {
            ResourceGroupName = resourceGroupName, 
            VirtualNetworkName = virtualNetworkName
        });
    }
}