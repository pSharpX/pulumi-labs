import { OneBankStackResolver, StackOutput, LinuxStackOutput } from "./app/OneBankStack";
import { Platform } from "./app/Platform";

const stack: StackOutput = new OneBankStackResolver().resolve(Platform.AL2023).build();

// Export the identifiers of the resources
export const instanceIp = stack.instanceIp
export const instanceHost = stack.instanceHost
export const command = (stack as LinuxStackOutput).command
export const stdout = (stack as LinuxStackOutput).stdout
