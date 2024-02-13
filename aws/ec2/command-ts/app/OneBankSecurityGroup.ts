import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";

type SecurityGroupIngress = aws.types.input.ec2.SecurityGroupIngress;

export class OneBankSecurityGroup {
    readonly sg: aws.ec2.SecurityGroup;

    constructor(resourceName: string, ingressRules?: pulumi.Input<SecurityGroupIngress>[]) {
        const config = new pulumi.Config();
        const tags = config.requireObject<object>("tags");
        const ingress = ingressRules || [];

        this.sg = new aws.ec2.SecurityGroup(`${resourceName}-sg`, {
            name: resourceName,
            description: "Inbound and outbound rules for network traffic",
            ingress: [
                { description: "Allow Inbound HTTP traffic", protocol: "tcp", fromPort: 80, toPort: 80, cidrBlocks: ["0.0.0.0/0"] },
                { description: "Allow Inbound HTTPS traffic", protocol: "tcp", fromPort: 443, toPort: 443, cidrBlocks: ["0.0.0.0/0"] },
                ...ingress
            ],
            egress: [
                { description: "Allow all outbound traffic", protocol: "-1", fromPort: 0, toPort: 0, cidrBlocks: ["0.0.0.0/0"] }
            ],
            tags: {
                ...tags,
                Name: resourceName
            }
        });
    }
}