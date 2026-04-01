namespace defaultapp;

public interface IInfrastructureResolver
{
    IComponentBuilder Resolve(AppComponentType appComponent);
}