using Pulumi;

namespace defaultapp.components;

public class FunctionAppComponent: ComponentResource
{
    public FunctionAppComponent(string name, FunctionAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:EnterpriseAppComponent", name, options)
    {
    }
}