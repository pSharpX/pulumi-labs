import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";

export enum Platform {
    UBUNTU = "ubuntu",
    AMAZON_LINUX = "amazon_linux",
    WINDOWS = "windows",
    WINDOWS_HOME = "windows-home",
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
            [Platform.WINDOWS_HOME]: new GetWindowsHomeAMI(),
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
        console.log("ubuntu resolver");
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["099720109477"],
            filters: [
                // {
                //     name: "platform",
                //     values: [
                //         "linux"
                //     ]
                // },
                {
                    name: "name",
                    values: [
                        "ubuntu/images/hvm-ssd/ubuntu-jammy-22.04-amd64-server-*" //ubuntu*
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
        console.log("amazon resolver");
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                // {
                //     name: "platform",
                //     values: [
                //         "linux"
                //     ]
                // },
                {
                    name: "name",
                    values: [
                        "amzn*"
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
        console.log("windows resolver");
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

export class GetWindowsHomeAMI implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        console.log("windows home resolver");
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
                        "Windows_Home-*"
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
        console.log("rhel resolver");
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                // {
                //     name: "platform",
                //     values: [
                //         "linux"
                //     ]
                // },
                {
                    name: "name",
                    values: [
                        "RHEL*"
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
