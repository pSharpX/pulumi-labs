using System.Collections.Generic;

namespace defaultapp.Factories;

public class CreateNetworkSecurityGroupArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public List<CreateSecurityRuleArgs> SecurityRules { get; init; } = [];
}