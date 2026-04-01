using System.Collections.Generic;
using Pulumi;

namespace defaultapp;

public class InfrastructureResult
{
    public ComponentResource? Application { get; init; }
    public Dictionary<string, object>? Output { get; init; }
}