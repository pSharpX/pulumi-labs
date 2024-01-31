using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws.Ec2;
using Pulumi.Aws.Ec2.Inputs;

public class OneBankStack: Pulumi.Stack
{
    public OneBankStack()
    {
        var config = new Config();
        var publicKey = config.Require("publicKey");
        var tags = config.RequireObject<Dictionary<string, string>>("tags");

        var allowHttpSG = new SecurityGroup("Allow_HTTP_SG", new SecurityGroupArgs
        {
            Name = "allow-http",
            Description = "Allow ingress HTTP traffic",
            Ingress = {
                new SecurityGroupIngressArgs {
                    Protocol = "tcp",
                    FromPort = 80,
                    ToPort = 80,
                    CidrBlocks = { "0.0.0.0/0" }
                }
            },
            Tags = tags
        });
        var allowSshSG = new SecurityGroup("Allow_SSH_SG", new SecurityGroupArgs
        {
            Name = "allow-ssh",
            Description = "Allow ingress SSH traffic",
            Ingress = {
                new SecurityGroupIngressArgs {
                    Protocol = "tcp",
                    FromPort = 22,
                    ToPort = 22,
                    CidrBlocks = { "0.0.0.0/0" }
                }
            },
            Tags = tags
        });
        var keyPair = new KeyPair("OneBank_KeyPair", new KeyPairArgs
        {
            KeyName = "ec2-csharp-kp",
            PublicKey = publicKey,
            Tags = tags
        });

        var server = new Instance("OneBank_EC2", new InstanceArgs
        {
            Ami = "ami-03c7d01cf4dedc891",
            InstanceType = "t2.micro",
            KeyName = keyPair.KeyName,
            VpcSecurityGroupIds = { allowHttpSG.Name, allowSshSG.Name },
            Tags = tags
        });

        InstanceIp = server.PublicIp;
        InstanceDns = server.PublicDns;
        Connection = server.PublicIp.Apply(publicIp => $"ssh -i ./ssh/ec2-keys ec2-user@{publicIp}");
    }

    [Output]
    public Output<string> InstanceIp {get; set;}
    [Output]
    public Output<string> InstanceDns {get; set;}
    [Output]
    public Output<string> Connection {get; set;}
}