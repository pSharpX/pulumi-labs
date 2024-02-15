import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import { OneBankInstance } from "./OneBankInstance";
import { OneBankKeyPair } from "./OneBankKeyPair";
import { OneBankSecurityGroup } from "./OneBankSecurityGroup";
import { OneBankProvisioner } from "./OneBankProvisioner";
import { Platform } from "./Platform";

type SecurityGroupIngress = aws.types.input.ec2.SecurityGroupIngress;

export interface StackOutput {
    instanceIp:  pulumi.Output<string>;
    instanceHost: pulumi.Output<string>;
}

export interface LinuxStackOutput extends StackOutput {
    command: pulumi.Output<string>;
    stdout: pulumi.Output<string>;
}

export interface WindowsStackOutput extends StackOutput {
    password: pulumi.Output<string>;
}

export interface StackBuilder {
    build(): StackOutput;
}

export class OneBankStackResolver {
    readonly resolvers: Record<Platform, StackBuilder> ;
    constructor() {
        this.resolvers = {
            [Platform.UBUNTU]: new UbuntuStackBuilder(),
            [Platform.WINDOWS]: new WindowsStackBuilder(),
            [Platform.WINDOWS_SERVER]: new WindowsStackBuilder(),
            [Platform.AMAZON_LINUX_2]: new AmazonLinuxStackBuilder(),
            [Platform.AL2023]: new AL2023StackBuilder(),
            [Platform.RHEL]: new UbuntuStackBuilder(),
        };
    }
    resolve(platform: Platform): StackBuilder {
        return this.resolvers[platform];
    }
}

export class UbuntuStackBuilder implements StackBuilder {
    readonly platform: Platform;
    readonly username: string;

    constructor() {
        this.platform = Platform.UBUNTU;
        this.username = "ubuntu";
    }
    build(): StackOutput {
        const config = new pulumi.Config();
        const env = config.require("environment");
        const applicationId = config.require("applicationId");
        const privateKey = config.requireSecret("privateKey");
        const resourceName = `${env}-${applicationId}`;
        const filename = "ubuntu_provisioner.sh";
        const source = `../config/${filename}`;
        const destination = `/tmp/${filename}`;
        const command = `chmod +x ${destination}; sudo ${destination};`;

        const ingressRules: pulumi.Input<SecurityGroupIngress>[] = [
            {
                description: "Allow Inbound SSH traffic",
                protocol: "tcp",
                fromPort: 22,
                toPort: 22,
                cidrBlocks: ["0.0.0.0/0"]
            }
        ];
        const securityGroupResource = new OneBankSecurityGroup(`${resourceName}api`, ingressRules);
        const keyPairResource = new OneBankKeyPair(resourceName);
        const instanceResource = new OneBankInstance(`${resourceName}api`, this.platform, [securityGroupResource.sg.id], keyPairResource.kp.id);
        const provisioner = new OneBankProvisioner(`${resourceName}prov`, instanceResource.instance, this.username, privateKey, source, destination, command);

        const output: LinuxStackOutput = {
            instanceIp: instanceResource.instance.publicIp,
            instanceHost: instanceResource.instance.publicDns,
            command: instanceResource.instance.publicIp.apply(publicIp => `ssh -i ./ssh/ec2-keys ${this.username}@${publicIp}`),
            stdout: provisioner.runScript.stdout
        };
        return output;
    }
}

export class AmazonLinuxStackBuilder implements StackBuilder {
    readonly platform: Platform;
    readonly username: string;
    
    constructor() {
        this.platform = Platform.AMAZON_LINUX_2;
        this.username = "ec2-user"
    }
    build(): StackOutput {
        const config = new pulumi.Config();
        const env = config.require("environment");
        const applicationId = config.require("applicationId");
        const privateKey = config.requireSecret("privateKey");
        const resourceName = `${env}-${applicationId}`;
        const filename = "amzn2_provisioner.sh";
        const source = `../config/${filename}`;
        const destination = `/tmp/${filename}`;
        const command = `chmod +x ${destination}; sudo ${destination}`;

        const ingressRules: pulumi.Input<SecurityGroupIngress>[] = [
            {
                description: "Allow Inbound SSH traffic",
                protocol: "tcp",
                fromPort: 22,
                toPort: 22,
                cidrBlocks: ["0.0.0.0/0"]
            }
        ];
        const securityGroupResource = new OneBankSecurityGroup(`${resourceName}api`, ingressRules);
        const keyPairResource = new OneBankKeyPair(resourceName);
        const instanceResource = new OneBankInstance(`${resourceName}api`, this.platform, [securityGroupResource.sg.id], keyPairResource.kp.id);
        const provisioner = new OneBankProvisioner(`${resourceName}prov`, instanceResource.instance, this.username, privateKey, source, destination, command);

        const output: LinuxStackOutput = {
            instanceIp: instanceResource.instance.publicIp,
            instanceHost: instanceResource.instance.publicDns,
            command: instanceResource.instance.publicIp.apply(publicIp => `ssh -i ./ssh/ec2-keys ${this.username}@${publicIp}`),
            stdout: provisioner.runScript.stdout
        };
        return output;
    }
}

export class AL2023StackBuilder implements StackBuilder {
    readonly platform: Platform;
    readonly username: string;
    
    constructor() {
        this.platform = Platform.AL2023;
        this.username = "ec2-user"
    }
    build(): StackOutput {
        const config = new pulumi.Config();
        const env = config.require("environment");
        const applicationId = config.require("applicationId");
        const privateKey = config.requireSecret("privateKey");
        const resourceName = `${env}-${applicationId}`;
        const filename = "al2023_provisioner.sh";
        const source = `../config/${filename}`;
        const destination = `/tmp/${filename}`;
        const command = `chmod +x ${destination}; sudo ${destination}`;

        const ingressRules: pulumi.Input<SecurityGroupIngress>[] = [
            {
                description: "Allow Inbound SSH traffic",
                protocol: "tcp",
                fromPort: 22,
                toPort: 22,
                cidrBlocks: ["0.0.0.0/0"]
            }
        ];
        const securityGroupResource = new OneBankSecurityGroup(`${resourceName}api`, ingressRules);
        const keyPairResource = new OneBankKeyPair(resourceName);
        const instanceResource = new OneBankInstance(`${resourceName}api`, this.platform, [securityGroupResource.sg.id], keyPairResource.kp.id);
        const provisioner = new OneBankProvisioner(`${resourceName}prov`, instanceResource.instance, this.username, privateKey, source, destination, command);

        const output: LinuxStackOutput = {
            instanceIp: instanceResource.instance.publicIp,
            instanceHost: instanceResource.instance.publicDns,
            command: instanceResource.instance.publicIp.apply(publicIp => `ssh -i ./ssh/ec2-keys ${this.username}@${publicIp}`),
            stdout: provisioner.runScript.stdout
        };
        return output;
    }
}

export class WindowsStackBuilder implements StackBuilder {
    readonly platform: Platform;
    constructor() {
        this.platform = Platform.WINDOWS_SERVER;
    }
    build(): WindowsStackOutput {
        const config = new pulumi.Config();
        const env = config.require("environment");
        const applicationId = config.require("applicationId");
        const resourceName = `${env}-${applicationId}`;

        const ingressRules: pulumi.Input<SecurityGroupIngress>[] = [
            {
                description: "Allow Inbound RDP traffic",
                protocol: "tcp",
                fromPort: 3389,
                toPort: 3389,
                cidrBlocks: ["0.0.0.0/0"]
            }
        ];
        const securityGroupResource = new OneBankSecurityGroup(`${resourceName}api`, ingressRules);
        const keyPairResource = new OneBankKeyPair(resourceName);
        const instanceResource = new OneBankInstance(`${resourceName}api`, this.platform, [securityGroupResource.sg.id], keyPairResource.kp.id);

        const output: WindowsStackOutput = {
            instanceIp: instanceResource.instance.publicIp,
            instanceHost: instanceResource.instance.publicDns,
            password: instanceResource.instance.passwordData
        };
        return output;
    }
}