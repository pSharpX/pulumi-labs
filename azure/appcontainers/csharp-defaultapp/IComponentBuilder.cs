using defaultapp.components;
using Pulumi;

namespace defaultapp;

public interface IComponentBuilder
{
    public InfrastructureResult Build(BuilderArgs args);
}