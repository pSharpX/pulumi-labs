import * as pulumi from "@pulumi/pulumi";
import * as forge from 'node-forge';

type PrivateKey = forge.pki.rsa.PrivateKey;

export class CryptoService {
    private privateKey: pulumi.Output<PrivateKey>;

    constructor() {
        const config = new pulumi.Config();
        const rawPrivateKey: pulumi.Output<string> = config.requireSecret("privateKey");
        this.privateKey = rawPrivateKey.apply(privateKey =>  forge.pki.privateKeyFromPem(privateKey));
    }

    decrypt(encryptedText: string): pulumi.Output<string> {
        const encrypted = forge.util.decode64(encryptedText);
        return this.privateKey.apply(key => key.decrypt(encrypted));
    }
}