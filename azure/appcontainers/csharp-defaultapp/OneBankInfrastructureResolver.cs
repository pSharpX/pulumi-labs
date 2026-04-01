using defaultapp.Builders;

namespace defaultapp;

public class OneBankInfrastructureResolver(
    DefaultAppComponentBuilder defaultAppComponentBuilder,
    LegacyAppComponentBuilder legacyAppComponentBuilder,
    FunctionAppComponentBuilder functionAppComponentBuilder,
    EnterpriseAppComponentBuilder enterpriseAppComponentBuilder,
    ServiceAppComponentBuilder serviceAppComponentBuilder,
    WebAppComponentBuilder webAppComponentBuilder)
    : IInfrastructureResolver
{
    public IComponentBuilder Resolve(AppComponentType appComponent)
    {
        switch (appComponent)
        {
            case AppComponentType.DefaultApp: return defaultAppComponentBuilder;
            case AppComponentType.LegacyApp: return legacyAppComponentBuilder;
            case AppComponentType.FunctionApp: return functionAppComponentBuilder;
            case AppComponentType.EnterpriseApp: return enterpriseAppComponentBuilder;
            case AppComponentType.ServiceApp: return serviceAppComponentBuilder;
            case AppComponentType.WebApp: return webAppComponentBuilder;
            default: throw new System.NotImplementedException();
        }
    }
}