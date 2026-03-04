using System.Collections.Generic;
using Pulumi;
using Pulumi.AzureNative.KeyVault;

namespace defaultapp.Factories;

public class CreateKeyArgs: CreateResourceArgs
{
    public required Input<string> VaultName { get; init; }
    public Input<bool> Enabled { get; init; } = false;
    public Input<double>? Expires { get; init; }
    public Input<bool> Exportable { get; init; } = false;
    public Input<double>? NotBefore { get; init; }
    public Input<string>? CurveName { get; init; } // The elliptic curve name. For valid values, see JsonWebKeyCurveName. Default for EC and EC-HSM keys is P-256
    public Input<int>? KeySize { get; init; }
    public Input<string>? KeyType { get; init; } // The type of the key. For valid values, see JsonWebKeyType. (EC, EC-HSM, RSA, RSA-HSM)

    public string[] KeyOps { get; init; } = []; // (encrypt, decrypt, sign, verify, wrapKey, unwrapKey, import, release)
}