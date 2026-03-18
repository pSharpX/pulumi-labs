using System;
using Pulumi;
using Pulumi.AzureNative.PrivateDns;
using Pulumi.AzureNative.PrivateDns.Inputs;

namespace defaultapp.Factories;

public static class PrivateRecordSetFactory
{
    public static PrivateRecordSet Create(CreateRecordSetArgs args)
    {
        PrivateRecordSetArgs recordSetArgs = new()
        {
            RecordType = args.RecordType,
            RelativeRecordSetName = args.Name,
            ResourceGroupName = args.ResourceGroupName,
            Ttl = args.Ttl,
            PrivateZoneName = args.ZoneName,
        };
        if (args.RecordType.Equals("A"))
        {
            if (string.IsNullOrEmpty(args.Ipv4Address)) throw new ArgumentNullException(nameof(args.Ipv4Address));
 
            recordSetArgs.ARecords = 
            [
                new ARecordArgs()
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
        return new PrivateRecordSet($"OneBank_RecordSet_{args.Alias}", recordSetArgs, new CustomResourceOptions { Parent = args.Parent });
    }
}