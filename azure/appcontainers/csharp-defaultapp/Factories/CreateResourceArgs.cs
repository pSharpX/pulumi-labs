using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public abstract class CreateResourceArgs
{
    public required Input<string> Name { get; init; }
    public required Input<string> ResourceGroupName { get; init; }
    public required Input<string>? Location { get; init; }
    public Dictionary<string, string>? Tags { get; init; }
    public ComponentResource? Parent { get; init; }
}