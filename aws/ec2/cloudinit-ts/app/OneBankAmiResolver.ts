import * as aws from "@pulumi/aws";

export enum Platform {
    UBUNTU = "ubuntu",
    AMAZON_LINUX_2 = "amazon_linux_2",
    AL2023 = "amazon_linux_2023",
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
            [Platform.AMAZON_LINUX_2]: new GetAmazonLinux2Ami(),
            [Platform.AL2023]: new GetAmazonLinux2023Ami(),
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

export class GetAmazonLinux2Ami implements AmiLookup {
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

export class GetAmazonLinux2023Ami implements AmiLookup {
    lookup(): Promise<aws.ec2.GetAmiResult> {
        const amis = aws.ec2.getAmi({
            mostRecent: true,
            owners: ["amazon"],
            filters: [
                {
                    name: "name",
                    values: [
                        "al2023-ami-2023*-x86_64",
                        "al2023-ami-minimal-2023*-x86_64"
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
                        "Windows_Server-2022-English-*-Base*"
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
