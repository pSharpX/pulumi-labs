using Pulumi;
using Pulumi.AzureNative.Sql;

namespace defaultapp.Factories;

public static class SqlServerFactory
{
    public static Server Create(CreateSqlServerArgs args)
    {
        return new Server($"OneBank_SqlServer_{args.Alias}", new ServerArgs
        {
            ServerName =  args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            AdministratorLogin = args.AdministratorLogin,
            AdministratorLoginPassword = args.AdministratorLoginPassword,
            IsIPv6Enabled = args.IsIPv6Enabled,
            MinimalTlsVersion =  args.MinimalTlsVersion,
            PublicNetworkAccess =  args.PublicNetworkAccess,
            Version =  args.Version,
            Tags = args.Tags!,
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}