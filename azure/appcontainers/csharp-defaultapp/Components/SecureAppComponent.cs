using Pulumi;

namespace defaultapp.components;

public class SecureAppComponent: ComponentResource
{
    public SecureAppComponent(string type, string name, ComponentResourceOptions? options = null) 
        : base(type, name, options)
    {
    }
}