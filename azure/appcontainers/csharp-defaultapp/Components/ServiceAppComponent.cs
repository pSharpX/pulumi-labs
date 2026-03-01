using Pulumi;

namespace defaultapp.components;

public class ServiceAppComponent: ComponentResource
{
    public ServiceAppComponent(string name, ServiceAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:ServiceAppComponent", name, options)
    {
    }
}