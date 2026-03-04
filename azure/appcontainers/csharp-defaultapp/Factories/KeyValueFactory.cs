using Pulumi;
using Pulumi.AzureNative.AppConfiguration;

namespace defaultapp.Factories;

public static class KeyValueFactory
{
    public static KeyValue Create(CreateKeyValueArgs args)
    {
        return new KeyValue($"OneBank_ConfigStore_KeyValue_{args.Alias}", new KeyValueArgs
        {
            ResourceGroupName =  args.ResourceGroupName,
            ConfigStoreName =  args.ConfigStoreName,
            KeyValueName = args.Name,
            ContentType =  args.ContentType,
            Value =   args.Value,
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent =  args.Parent });
    }
}