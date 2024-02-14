import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import { Platform } from "./Platform";
import { OneBankImageResolver } from "./OneBankImageResolver";

export class OneBankInstance {
    readonly instance: aws.ec2.Instance;

    constructor(resourceName: string, platform: Platform, securityGroups: pulumi.Input<string>[], keyPairId?: pulumi.Input<string>) {
        const config = new pulumi.Config();
        const tags = config.requireObject<object>("tags");
        const windowsPlatform = [Platform.WINDOWS, Platform.WINDOWS_SERVER];

        const imageId = new OneBankImageResolver().resolve(platform);
        this.instance = new aws.ec2.Instance(`${resourceName}-instance`, {
            ami: imageId,
            instanceType:  windowsPlatform.includes(platform) ? "t3.large": "t2.micro",
            keyName: keyPairId,
            vpcSecurityGroupIds: securityGroups,
            getPasswordData: windowsPlatform.includes(platform),
            tags: {
                ...tags,
                Name: resourceName
            }            
        });
    }
}