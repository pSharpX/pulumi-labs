using Pulumi;

namespace defaultapp.components;

public class EnterpriseAppComponent: ComponentResource
{
    public EnterpriseAppComponent(string name, ComponentResourceOptions? options = null) 
        : base("custom:components:EnterpriseAppComponent", name, options)
    {
    }
}