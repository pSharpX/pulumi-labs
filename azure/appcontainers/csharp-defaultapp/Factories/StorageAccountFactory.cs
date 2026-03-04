using System;
using Pulumi;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace defaultapp.Factories;

public static class StorageAccountFactory
{
    public static StorageAccount Create(CreateStorageAccountArgs args)
    {
        ImmutableStorageAccountArgs? immutableStorageSettings = null;
        if (args.ImmutableStorageEnabled)
        {
            immutableStorageSettings = new ImmutableStorageAccountArgs
            {
                Enabled =  true,
            };
        }
        return new StorageAccount("OneBank_StorageAccount", new StorageAccountArgs
        {
            AccountName =  args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Kind = args.Kind,
            Sku = new SkuArgs
            {
                Name = args.SkuName,
            },
            MinimumTlsVersion = args.MinimumTlsVersion,
            EnableHttpsTrafficOnly = args.EnableHttpsTrafficOnly,
            AccessTier =  Enum.Parse<AccessTier>(args.AccessTier),
            PublicNetworkAccess = args.PublicNetworkAccess,
            AllowBlobPublicAccess = args.AllowBlobPublicAccess,
            AllowSharedKeyAccess = args.AllowSharedKeyAccess,
            ImmutableStorageWithVersioning = immutableStorageSettings!,
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}