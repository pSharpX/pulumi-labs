using System.Collections.Generic;
using Pulumi;

namespace defaultapp;

public class InfrastructureResult
{
    public ComponentResource? Application { get; init; }
    public required Output<Dictionary<string, string>> Output { get; init; }
}