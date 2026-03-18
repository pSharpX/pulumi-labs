using System;
using Pulumi;
using Pulumi.AzureNative.Dns;
using Pulumi.AzureNative.Dns.Inputs;

namespace defaultapp.Factories;

public static class RecordSetFactory
{
    public static RecordSet Create(CreateRecordSetArgs args)
    {
        RecordSetArgs recordSetArgs = new()
        {
            RecordType = args.RecordType,
            RelativeRecordSetName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Ttl = args.Ttl,
            ZoneName = args.ZoneName,
        };
        if (args.RecordType.Equals("A"))
        {
            if (string.IsNullOrEmpty(args.Ipv4Address)) throw new ArgumentNullException(nameof(args.Ipv4Address));
 
            recordSetArgs.ARecords = 
            [
                new ARecordArgs
                {
                    Ipv4Address =  args.Ipv4Address!,
                }    
            ];
        }
        if (args.RecordType.Equals("AAAA"))
        {
            if (string.IsNullOrEmpty(args.Ipv6Address)) throw new ArgumentNullException(nameof(args.Ipv6Address));
 
            recordSetArgs.AaaaRecords = 
            [
                new AaaaRecordArgs()
                {
                    Ipv6Address =  args.Ipv6Address!,
                }    
            ];
        }
        if (args.RecordType.Equals("CNAME"))
        {
            if (string.IsNullOrEmpty(args.Cname)) throw new ArgumentNullException(nameof(args.Cname));
 
            recordSetArgs.CnameRecord = new CnameRecordArgs()
            {
                Cname =  args.Cname!,
            };
        }
        if (args.RecordType.Equals("NS"))
        {
            if (string.IsNullOrEmpty(args.Nsdname)) throw new ArgumentNullException(nameof(args.Nsdname));
 
            recordSetArgs.NsRecords = new NsRecordArgs()
            {
                Nsdname =  args.Nsdname!,
            };
        }
        return new RecordSet($"OneBank_RecordSet_{args.Alias}", recordSetArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}