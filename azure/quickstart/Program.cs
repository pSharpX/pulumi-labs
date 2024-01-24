using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using System.Collections.Generic;

return await Pulumi.Deployment.RunAsync(() =>
{
    // Tags to be used for provisioned resources
    var applicationId = "onebank";
    var resourceGroupName = "TeamGOAT_rg";
    var storageAccountName = $"{applicationId}sa";
    
    InputMap<string> inputTags = new InputMap<string>
    {
        { "ApplicationId", applicationId },
        { "TechnicalOwner", "TeamGOAT" },
        { "Environment", "Development" },
        { "Provisioner", "Pulumi" },
        { "DataClassification", "Restricted" }
    };

    // Create an Azure Resource Group
    var resourceGroup = new ResourceGroup(resourceGroupName, new ResourceGroupArgs
    {
        ResourceGroupName = resourceGroupName,
        Tags = inputTags
    });

    // Create an Azure resource (Storage Account)
    var storageAccount = new StorageAccount(storageAccountName, new StorageAccountArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccountName,
        Sku = new SkuArgs
        {
            Name = SkuName.Standard_LRS
        },
        Kind = Kind.StorageV2,
        AccessTier = AccessTier.Hot,
        EnableHttpsTrafficOnly = true,
        Tags = inputTags
    });

    var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
    {
        ResourceGroupName = resourceGroup.Name,
        AccountName = storageAccount.Name
    });

    var primaryStorageKey = storageAccountKeys.Apply(accountKeys =>
    {
        var firstKey = accountKeys.Keys[0].Value;
        return Output.CreateSecret(firstKey);
    });

    // Export the primary key of the Storage Account
    return new Dictionary<string, object?>
    {
        ["primaryStorageKey"] = primaryStorageKey
    };
});