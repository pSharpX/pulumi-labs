using System.Collections.Generic;

namespace defaultapp.Factories;

public class CreateApplicationGatewayArgs: CreateResourceArgs
{
    public string SkuName { get; init; } = "Basic"; // Standard_Small, Standard_Medium, Standard_Large, WAF_Medium, WAF_Large, Standard_v2, WAF_v2, Basic
    public string SkuTier { get; init; } = "Basic"; // Basic, Standard, WAF, Standard_v2, WAF_v2
    public int SkuCapacity { get; init; } = 1;
    public string SkuFamily { get; init; } = "Generation_2"; // Generation_1, Generation_2
    public bool EnableHttp2 { get; init; } = false;
    

    public List<(string, string)> GatewayIpConfigurations { get; init; } = [];
    public List<(string, int)> FrontendPorts { get; init; } = [];
    public List<(string, string)> FrontendIPConfigurations { get; init; } = [];
    public List<(string, List<string>)> BackendAddressPools { get; init; } = [];
    public List<(string, int, string, bool, int)> BackendHttpSettingsCollection { get; init; } = [];
    public List<(string, string, string, string)> HttpListeners { get; init; } = [];
    public List<(string, string, string, string, string, int)> RequestRoutingRules { get; init; } = [];
}