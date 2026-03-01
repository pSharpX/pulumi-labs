using Pulumi;

namespace defaultapp.Factories;

public class CreateKeyValueArgs: CreateResourceArgs
{
    public required Input<string> ConfigStoreName { get; init; }
    public required Input<string> Value { get; init; }
    public required Input<string> Label { get; init; }
    public Input<string>? ContentType { get; init; }
}