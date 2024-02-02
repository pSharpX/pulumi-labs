import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";

export class OneBankInstance {
    readonly instance: aws.ec2.Instance;

    constructor(resourceName: string, keyPairId: pulumi.Input<string>, securityGroups: pulumi.Input<string>[]) {
        const config = new pulumi.Config();
        const tags = config.requireObject<object>("tags");

        this.instance = new aws.ec2.Instance(`${resourceName}-instance`, {
            ami: "ami-03c7d01cf4dedc891",
            instanceType: "t2.micro",
            keyName: keyPairId,
            vpcSecurityGroupIds: securityGroups,
            tags: {
                ...tags,
                Name: resourceName
            }            
        });
    }
}