import * as pulumi from "@pulumi/pulumi";
import * as aws from "@pulumi/aws";
import * as path from "path";
import { remote, types } from "@pulumi/command";
import { getFileHash } from "./OneBankUtils";

type ConnectionArgs = types.input.remote.ConnectionArgs;

export class OneBankProvisioner {
    private readonly connection: ConnectionArgs;
    readonly cpScript: remote.CopyFile;
    readonly runScript: remote.Command;
 
    constructor(resourceName: string, server: aws.ec2.Instance, user: string, privateKey: pulumi.Output<string>, source: string, destination: string, command: string) {
        this.connection = {
            host: server.publicIp,
            privateKey,
            user
        };
        const changeScript = getFileHash(source);
        this.cpScript = new remote.CopyFile(`${resourceName}copy`, {
            connection: this.connection,
            triggers: [changeScript],
            localPath: path.join(__dirname, source),
            remotePath: destination
        }, { 
            dependsOn: server
        });
        this.runScript = new remote.Command(`${resourceName}runner`, {
            connection: this.connection,
            triggers: [changeScript],
            create: command
        }, {
            dependsOn: this.cpScript
        });
    }
}