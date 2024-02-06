import { OneBankInstance } from "./app/OneBankInstance";
import { OneBankKeyPair } from "./app/OneBankKeyPair";
import { OneBankSecurityGroup } from "./app/OneBankSecurityGroup";
import { Platform } from "./app/OneBankAmiResolver";

const securityGroupResource = new OneBankSecurityGroup("dev-onebankapi");
const keyPairResource = new OneBankKeyPair("dev-onebank");
const instanceResource = new OneBankInstance("dev-onebankapi", Platform.AMAZON_LINUX, keyPairResource.kp.id, [securityGroupResource.sg.id]);

// Export the identifiers of the resources
export const instanceIp = instanceResource.instance.publicIp;
export const instanceDns = instanceResource.instance.publicDns;
export const sshCommand = instanceResource.instance.publicIp.apply(publicIp => `ssh -i ./ssh/ec2-keys ec2-user@${publicIp}`);
