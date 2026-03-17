using System.Collections.Generic;

namespace defaultapp.Factories;

public class CreateSecurityRuleArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public string? Description { get; init; }
    public required string Access { get; init; } // Deny, Allow
    public string? DestinationAddressPrefix { get; init; }
    public List<string>? DestinationAddressPrefixes { get; init; }
    public List<string>? DestinationApplicationSecurityGroups { get; init; } = null;
    public string? DestinationPortRange { get; init; }
    public List<string>? DestinationPortRanges { get; init; }
    public required string Direction { get; init; } // Outbound, Inbound
    public string? NetworkSecurityGroupName { get; init; }
    public int Priority { get; init; } = 100;
    public string Protocol { get; init; } = "Tcp"; // Tcp, Udp, Icmp, Esp, *, Ah
    public string? SourceAddressPrefix { get; init; }
    public List<string>? SourceAddressPrefixes { get; init; }
    public List<string>? SourceApplicationSecurityGroups { get; init; } = null;
    public string SourcePortRange { get; init; } = "*";
    public List<string>? SourcePortRanges { get; init; }
}