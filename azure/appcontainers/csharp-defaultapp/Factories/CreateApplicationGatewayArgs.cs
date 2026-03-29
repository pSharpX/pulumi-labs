using System.Collections.Generic;
using Pulumi;

namespace defaultapp.Factories;

public class CreateApplicationGatewayArgs: CreateResourceArgs
{
    public string SkuName { get; init; } = "Basic"; // Basic, Standard_Small, Standard_Medium, Standard_Large, WAF_Medium, WAF_Large, Standard_v2, WAF_v2
    public string SkuTier { get; init; } = "Basic"; // Basic, Standard, WAF, Standard_v2, WAF_v2
    public int SkuCapacity { get; init; } = 1;
    public string SkuFamily { get; init; } = "Generation_2"; // Generation_1, Generation_2
    public bool EnableHttp2 { get; init; } = false;
    public bool EnableAutoScale { get; init; } = false;
    public int MinCapacity { get; init; } = 1;
    public int MaxCapacity { get; init; } = 2;
    public InputList<(string, string)> GatewayIpConfigurations { get; init; } = [];
    public List<(string, int)> FrontendPorts { get; init; } = [];
    public List<(string, string)> FrontendIPConfigurations { get; init; } = [];
    public InputList<(string, List<string>)> BackendAddressPools { get; init; } = [];
    public List<(string, int, string, bool, int)> BackendHttpSettingsCollection { get; init; } = [];
    public List<(string, string, string, string)> HttpListeners { get; init; } = [];
    public List<(string, string, string, string, string, int)> RequestRoutingRules { get; init; } = [];
    public Input<string>? SubnetId { get; init; }
    public int Port { get; init; } = 80;
    public int BackendPort { get; init; } = 80;
    public InputList<string>BackendFqdn { get; set; } = [];
    public Input<string>? BackendHostname { get; set; }
    public bool PickHostNameFromBackendAddress { get; set; } = true;
    public Input<string>? PublicIpAddressId { get; init; }
    public int RequestTimeout { get; init; } = 60;
    public string Path { get; init; } = "/";
    public string Protocol { get; init; } = "Http"; // Http, Https, Tcp, Tls
    public string BackendProtocol { get; init; } = "Http";
    public string RoutingRule { get; init; } = "Basic"; // Basic, PathBasedRouting
    public int Priority { get; init; } = 1;
}