using defaultapp;
using Pulumi;

//return await Deployment.RunAsync<OneBankStack>();
//return await Deployment.RunAsync<OneBankComponentStack>();
return await Deployment.RunAsync<OneBankInfrastructureStack>();