namespace defaultapp;

public enum AppComponentType
{
    DefaultApp, // containerized application running on container apps
    EnterpriseApp, // application running on a microservice architecture/pattern 
    LegacyApp, // application running on virtual machine
    ServiceApp, // application running on aks
    WebApp, // long-lived application running on app service
    StaticWebApp, // Static application hosted on Storage Account  
    FunctionApp, // serverless application or function
    WorkerApp, // run-to-completion application running on container instance
}