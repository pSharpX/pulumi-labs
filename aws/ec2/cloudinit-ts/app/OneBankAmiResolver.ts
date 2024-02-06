import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";

export enum Platform {
    UBUNTU = "ubuntu",
    AMAZON_LINUX = "amazon_linux",
    WINDOWS = "windows",
    WINDOWS_SERVER = "windows_server",
    RHEL = "rhel"
}

export interface AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult>;
}

export class OneBankAmiResolver {
    readonly resolvers: Record<Platform, AmiLookup> ;
    constructor() {
        this.resolvers = {
            [Platform.UBUNTU]: new GetUbuntuAmi(),
            [Platform.WINDOWS]: new GetWindowsAMI(),
            [Platform.WINDOWS_SERVER]: new GetWindowsServerAMI(),
            [Platform.AMAZON_LINUX]: new GetAmazonAmi(),
            [Platform.RHEL]: new GetRHELAMI(),
        };
    }
    resolve(platform: Platform): AmiLookup {
        return this.resolvers[platform];
    }
}

export class GetUbuntuAmi implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["099720109477"],
            
            filters: [
                {
                    name: "name",
                    values: [
                        "ubuntu/images/hvm-ssd/ubuntu-jammy-22.04-amd64-server-*"
                    ]
                },
                {
                    name: "virtualization-type",
                    values: [
                        "hvm"
                    ]
                },
                {
                    name: "architecture",
                    values: [
                        "x86_64"
                    ]
                },
            ]
        });
        return amis;
    }
}

export class GetAmazonAmi implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                {
                    name: "name",
                    values: [
                        "amzn2-ami-hvm*-x86_64-gp2"
                    ]
                },
                {
                    name: "virtualization-type",
                    values: [
                        "hvm"
                    ]
                },
                {
                    name: "architecture",
                    values: [
                        "x86_64"
                    ]
                }
            ]
        });
        return amis;
    }
}

export class GetWindowsAMI implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                {
                    name: "platform",
                    values: [
                        "windows"
                    ]
                },
                {
                    name: "virtualization-type",
                    values: [
                        "hvm"
                    ]
                },
                {
                    name: "architecture",
                    values: [
                        "x86_64"
                    ]
                }
            ]
        });
        return amis;
    }
}

export class GetWindowsServerAMI implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                {
                    name: "platform",
                    values: [
                        "windows"
                    ]
                },
                {
                    name: "name",
                    values: [
                        "Windows_Server-2022*"
                    ]
                },
                {
                    name: "virtualization-type",
                    values: [
                        "hvm"
                    ]
                },
                {
                    name: "architecture",
                    values: [
                        "x86_64"
                    ]
                }
            ]
        });
        return amis;
    }
}

export class GetRHELAMI implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                {
                    name: "name",
                    values: [
                        "RHEL_8.5-x86_64-*"
                    ]
                },
                {
                    name: "virtualization-type",
                    values: [
                        "hvm"
                    ]
                },
                {
                    name: "architecture",
                    values: [
                        "x86_64"
                    ]
                }
            ]
        });
        return amis;
    }
}
