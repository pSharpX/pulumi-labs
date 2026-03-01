using Pulumi;
using Pulumi.AzureNative.AppConfiguration;

namespace defaultapp.Factories;

public static class KeyValueFactory
{
    public static KeyValue Create(CreateKeyValueArgs args)
    {
        return new KeyValue($"OneBank_KeyValue_{args.Label}", new KeyValueArgs
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