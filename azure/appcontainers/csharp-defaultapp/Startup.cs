using defaultapp.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace defaultapp;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IInfrastructureResolver, OneBankInfrastructureResolver>();

        services.AddSingleton<DefaultAppComponentBuilder>();
        services.AddSingleton<LegacyAppComponentBuilder>();
        services.AddSingleton<FunctionAppComponentBuilder>();
        services.AddSingleton<EnterpriseAppComponentBuilder>();
        services.AddSingleton<ServiceAppComponentBuilder>();
        services.AddSingleton<WebAppComponentBuilder>();
    }
}