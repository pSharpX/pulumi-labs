import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import * as awsx from "@pulumi/awsx";

const config = new pulumi.Config();
const tags = config.requireObject<object>("tags");
const publicKey = config.require("publicKey");

const keyPair = new aws.ec2.KeyPair("OneBank_KeyPair", {
    keyName: "ec2-ts-kp",
    publicKey: publicKey,
    tags: {
        ...tags,
        Name: "ec2-ts-kp"
    }
});

const securityGroups = new aws.ec2.SecurityGroup("OneBank_SG", {
    name: "onebank-ec2-sg",
    description: "Ingress and Egress rules for network traffict",
    egress: [
        {
            description: "Allow all outbound traffict",
            fromPort: 0,
            toPort: 0,
            protocol: "-1",
            cidrBlocks: ["0.0.0.0/0"]
        }
    ],
    ingress: [
        {
            description: "Allow HTTP",
            protocol: "tcp",
            fromPort: 80,
            toPort: 80,
            cidrBlocks: ["0.0.0.0/0"]
        }, {
            description: "Allow HTTPS",
            protocol: "tcp",
            fromPort: 443,
            toPort: 443,
            cidrBlocks: ["0.0.0.0/0"]
        }, {
            description: "Allow SSH",
            protocol: "tcp",
            fromPort: 22,
            toPort: 22,
            cidrBlocks: ["0.0.0.0/0"]
        }
    ],
    tags: {
        ...tags,
        Name: "onebank-ec2-sg"
    }
});

const instance = new aws.ec2.Instance("OneBank_EC2", {
    ami: "ami-03c7d01cf4dedc891",
    instanceType: "t2.micro",
    keyName: keyPair.id,
    vpcSecurityGroupIds: [securityGroups.id],
    tags: {
        ...tags,
        Name: "onebank-ec2"
    }
});

export const instanceIp = instance.publicIp;
export const instanceDns = instance.publicDns;
export const connection = instance.publicIp.apply(publicIp => `ssh -i ./ssh/ec2-keys ec2-user@${publicIp}`);
