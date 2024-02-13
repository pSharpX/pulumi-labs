import * as pulumi from "@pulumi/pulumi";
import { Platform } from "./Platform";

type GetImageResult = {
    readonly imageId: string;
    readonly imageLocation: string;
}

export class OneBankImageResolver {
    readonly resolvers: Record<Platform, GetImageResult> ;
    constructor() {
        this.resolvers = {
            [Platform.UBUNTU]: {
                imageId: "ami-07d9b9ddc6cd8dd30",
                imageLocation: "amazon/ubuntu/images/hvm-ssd/ubuntu-jammy-22.04-amd64-server-20240207.1"  
            },
            [Platform.WINDOWS]: {
                imageId: "ami-00d990e7e5ece7974",
                imageLocation: "amazon/Windows_Server-2022-English-Full-Base-2024.01.16"  
            },
            [Platform.WINDOWS_SERVER]: {
                imageId: "ami-00d990e7e5ece7974",
                imageLocation: "amazon/Windows_Server-2022-English-Full-Base-2024.01.16"
            },
            [Platform.AMAZON_LINUX_2]: {
                imageId: "ami-03c951bbe993ea087",
                imageLocation: "amazon/amzn2-ami-hvm-2.0.20240131.0-x86_64-gp2"
            },
            [Platform.AL2023]: {
                imageId: "ami-02839f5510d78d6ef",
                imageLocation: "amazon/al2023-ami-minimal-2023.3.20240205.2-kernel-6.1-x86_64"
            },
            [Platform.RHEL]: {
                imageId: "ami-0e8f9bb800a080518",
                imageLocation: "amazon/RHEL_8.5-x86_64-SQL_2017_Enterprise-2022.10.06"
            },
        };
    }
    resolve(platform: Platform): pulumi.Input<string> {
        return  this.resolvers[platform].imageId;
    }
}