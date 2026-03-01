using Pulumi;

namespace defaultapp.components;

public class WebAppComponent: ComponentResource
{
    public WebAppComponent(string name, WebAppComponentArgs args, ComponentResourceOptions? options = null) 
        : base("custom:components:WebAppComponent", name, options)
    {
    }
}