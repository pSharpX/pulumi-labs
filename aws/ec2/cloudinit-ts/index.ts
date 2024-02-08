import { LinuxStackOutput, OneBankStackResolver, StackOutput, WindowsStackOutput } from "./app/OneBankStack";
import { Platform } from "./app/OneBankAmiResolver";

const stack: StackOutput = new OneBankStackResolver().resolve(Platform.WINDOWS).build();

// Export the identifiers of the resources
export const instanceIp = stack.instanceIp
export const instanceHost = stack.instanceHost
//export const command = (stack as LinuxStackOutput).command
export const password = (stack as WindowsStackOutput).password
