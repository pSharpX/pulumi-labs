import * as forge from 'node-forge';
import * as fs from "fs";
import * as path from "path";

type PrivateKey = forge.pki.rsa.PrivateKey;

export class CryptoService {
    private privateKey: PrivateKey;

    constructor() {
        const keyPath: string = "../../../../ssh/ec2-keys";
        const rawPrivateKey = fs.readFileSync(path.join(__dirname, keyPath), 'utf8')
        this.privateKey = forge.pki.privateKeyFromPem(rawPrivateKey);
    }

    decrypt(encryptedText: string): string {
        const encrypted = forge.util.decode64(encryptedText);
        return this.privateKey.decrypt(encrypted);
    }
}