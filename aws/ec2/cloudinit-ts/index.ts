import { LinuxStackOutput, OneBankStackResolver, StackOutput, WindowsStackOutput } from "./app/OneBankStack";
import { Platform } from "./app/OneBankAmiResolver";
import { CryptoService } from "./app/CrypoService";

const crytoService = new CryptoService();
const stack: StackOutput = new OneBankStackResolver().resolve(Platform.WINDOWS_SERVER).build();

// Export the identifiers of the resources
export const instanceIp = stack.instanceIp
export const instanceHost = stack.instanceHost
//export const command = (stack as LinuxStackOutput).command
export const password = (stack as WindowsStackOutput).password.apply(encryptedPass => crytoService.decrypt(encryptedPass))
