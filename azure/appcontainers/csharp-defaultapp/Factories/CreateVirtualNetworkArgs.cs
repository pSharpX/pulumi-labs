using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateVirtualNetworkArgs: CreateResourceArgs
{ 
    public required InputList<string> AddressPrefixes { get; init; }
    public List<CreateSubnetArgs>? Subnets { get; init; }
}