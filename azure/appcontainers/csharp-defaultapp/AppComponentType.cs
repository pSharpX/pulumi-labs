namespace defaultapp;

public enum AppComponentType
{
    DefaultApp, // application running on container apps
    EnterpriseApp, // application running on a microservice architecture/pattern 
    LegacyApp, // application running on virtual machine
    ServiceApp, // application running on aks
    WebApp, // long-lived application running on app service
    FunctionApp, // serverless application or function
}