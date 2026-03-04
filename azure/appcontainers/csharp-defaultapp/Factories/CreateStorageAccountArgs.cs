using Pulumi;

namespace defaultapp.Factories;

public class CreateStorageAccountArgs: CreateResourceArgs
{
    public Input<bool> AllowBlobPublicAccess { get; init; } = false;
    public Input<bool> AllowSharedKeyAccess { get; init; } = false;
    public bool ImmutableStorageEnabled { get; init; } = false;
    public bool EncryptionEnabled { get; init; } = false;
    public Input<bool> EnableHttpsTrafficOnly { get; init; } = true;
    public Input<string> Kind { get; init; } = "StorageV2"; //AzureNative.Storage.Kind.Storage (Storage, StorageV2, BlobStorage, FileStorage, BlockBlobStorage)
    public Input<string> PublicNetworkAccess { get; init; } = "Enabled"; //AzureNative.Storage.PublicNetworkAccess.Enabled
    public Input<string> SkuName { get; init; } =  "Standard_GRS"; // AzureNative.Storage.SkuName.Standard_GRS
    public string AccessTier { get; init; } = "Hot"; //AzureNative.Storage.AccessTier.Hot
    public Input<string> MinimumTlsVersion { get; init; } = "TLS1_2"; //AzureNative.Storage.MinimumTlsVersion.TLS1_2 (TLS1_0, TLS1_1, TLS1_2, TLS1_3)
}