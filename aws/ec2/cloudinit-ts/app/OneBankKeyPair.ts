import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";

export class OneBankKeyPair {
    readonly kp: aws.ec2.KeyPair;

    constructor(resourceName: string) {
        const config = new pulumi.Config();
        const tags = config.requireObject<object>("tags");
        const publicKey = config.require("publicKey");

        this.kp = new aws.ec2.KeyPair(`${resourceName}-kp`, {
            keyName: resourceName,
            publicKey: publicKey,
            tags: {
                ...tags,
                Name: resourceName
            }
        });
    }
}