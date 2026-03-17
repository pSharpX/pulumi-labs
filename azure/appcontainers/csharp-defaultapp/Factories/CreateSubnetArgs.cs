using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateSubnetArgs: CreateResourceArgs
{
    public required string Alias { get; init;  }
    public required string SubnetName { get; init;  }
    public required Input<string> VirtualNetworkName { get; init;  }
    public Input<string>? SubnetAddressPrefix { get; init; }
    public InputList<string>? SubnetAddressPrefixes { get; init; }
    public List<(string, string)>? Delegations { get; init; }
    public Input<string>? NetworkSecurityGroupId { get; init; }
}