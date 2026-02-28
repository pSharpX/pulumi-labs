using Pulumi;

namespace defaultapp.components;

public class PrivateAppComponent: ComponentResource
{
    public PrivateAppComponent(string type, string name, ComponentResourceOptions? options = null) 
        : base(type, name, options)
    {
    }
}