using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateVirtualNetworkArgs: CreateResourceArgs
{ 
    public required InputList<string> AddressPrefixes { get; init; }
    public required InputList<string> SubnetAddressPrefixes { get; init; }
}