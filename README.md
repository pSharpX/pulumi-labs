# pulumi-labs
handy set of recipes for provisioning infra using Pulumi

# Commands
pulumi --help
pulumi <command> --help

pulumi version

# Pulumi Cloud Commands:
pulumi login
pulumi logout

pulumi new azure-csharp
# Pulumi IA - flag: must be one of TypeScript, JavaScript, Python, Go, C#, Java, or YAML
pulumi new --language csharp
pulumi stack

# Create or update the resources in a stack.
pulumi up
# Automatically approve and perform the update after previewing it
pulumi up -y

pulumi up --diff
pulumi preview

# --skip-preview ( Do not calculate a preview before performing the update)
pulumi up -f 

# --stack string ( The name of the stack to operate on. Defaults to the current stack)
pulumi up -s <stack_name>

# You can access your outputs from the CLI by running
pulumi stack output [property-name]
pulumi stack output primaryStorageKey

# Destroy resources
pulumi destroy
pulumi destroy -y

# Delete this Stack's Resources
pulumi destroy -s psharpx/quickstart/dev

# Delete this Stack
pulumi stack rm psharpx/quickstart/dev

# csharp commands
dotnet add package Pulumi.Docker


dotnet list SentimentAnalysis.csproj package
dotnet list package --format json