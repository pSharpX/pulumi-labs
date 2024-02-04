import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import { Platform, OneBankAmiResolver } from "./OneBankAmiResolver";

export class OneBankInstance {
    readonly instance: aws.ec2.Instance;

    constructor(resourceName: string, platform: Platform, keyPairId: pulumi.Input<string>, securityGroups: pulumi.Input<string>[]) {
        const config = new pulumi.Config();
        const tags = config.requireObject<object>("tags");

        const ami = new OneBankAmiResolver().resolve(platform).lookup();
        this.instance = new aws.ec2.Instance(`${resourceName}-instance`, {
            ami: ami.then(ubuntu => {
                console.log(ubuntu);
                return ubuntu.id;
            }),
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