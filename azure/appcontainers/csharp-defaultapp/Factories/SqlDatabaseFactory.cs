using Pulumi;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace defaultapp.Factories;

public static class SqlDatabaseFactory
{
    public static Database Create(CreateSqlDatabaseArgs args)
    {
        return new Database($"OneBank_SqlDatabase_{args.Alias}", new DatabaseArgs
        {
            DatabaseName = args.Name,
            ServerName = args.ServerName,
            ResourceGroupName = args.ResourceGroupName,
            Location = args.Location,
            Collation = "SQL_Latin1_General_CP1_CI_AI",
            LicenseType = args.LicenseType,
            SampleName = args.SampleName,
            Sku = new SkuArgs
            {
                Capacity = args.SkuCapacity,
                Family = args.SkuFamily,
                Name = args.SkuName, /*Serverless*/
            }    
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}