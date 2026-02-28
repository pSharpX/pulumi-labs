using Pulumi;

namespace defaultapp.components;

public class EnterpriseAppComponent: ComponentResource
{
    public EnterpriseAppComponent(string type, string name, ComponentResourceOptions? options = null) 
        : base(type, name, options)
    {
    }
}