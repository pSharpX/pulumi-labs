namespace defaultapp.Factories;

public class CreateRecordSetArgs: CreateResourceArgs
{
    public required string Alias { get; init; }
    public string? Ipv4Address { get; init; }
    public string? Ipv6Address { get; init; }
    public string? Cname { get; init; }
    public string? Nsdname { get; init; }
    public required string RecordType { get; init; } = "A"; // A, AAAA, CAA, CNAME, DS, MX, NS, SOA
    public required string ZoneName { get; init; }
    public int Ttl { get; init; } = 3600;
}