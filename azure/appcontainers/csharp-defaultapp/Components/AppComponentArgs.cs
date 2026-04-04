using System.Collections.Generic;
using Pulumi;

namespace defaultapp.components;

public abstract class AppComponentArgs
{
    public required Input<string> ClientId { get; init; }
    public required Input<string> ObjectId { get; init; }
    public required Input<string> TenantId { get; init; }
    public required Input<string> SubscriptionId { get; init; }
    public required Input<string> Name { get; init; }
    public required Input<string> ParentName { get; init; }
    public required Input<string> ResourceGroupName { get; init; }
    public Input<string>? Location { get; init; }
    public string Environment { get; init; } = "dev"; // dev, qa, staging, prod
    public Dictionary<string, string>? Tags { get; init; }
    public bool Private { get; init; }
    public bool External { get; init; } = true;
    public Input<bool> EnableScaling { get; init; } = false;
    public Input<int> MinInstances { get; init; } = 1;   
    public Input<int> MaxInstances { get; init; } = 1;
    public bool EnableVault { get; init; }
    public string? VaultName { get; init; }
    public bool EnableConfigStore { get; init; }
    public string? ConfigStoreName { get; init; }
    public bool EnableStorage { get; init; }
    public string? StorageAccountName { get; init; }
    public bool EnableRegistry { get; init; }
    public string? RegistryName { get; init; }
    public bool EnableEncryption { get; init; }
    public string? EncryptionKeyName { get; init; }
    public bool EnableDatabase { get; init; }
    public string DatabaseEngine { get; init; } = "mssql"; // mssql, psql, mysql
}