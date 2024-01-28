# pulumi-labs
handy set of recipes for provisioning infra using Pulumi

# Commands
pulumi --help
pulumi <command> --help

pulumi version

# Pulumi Cloud Commands:
pulumi login
pulumi logout

# Pulumi IA - flag: must be one of TypeScript, JavaScript, Python, Go, C#, Java, or YAML
pulumi new
pulumi new csharp
pulumi new aws-csharp
pulumi new azure-csharp
pulumi new gcp-csharp
pulumi new visualbasic
pulumi new java
pulumi stack

## This csharp template is cloud agnostic, and you will need to install NuGet packages for the cloud provider of your choice
pulumi new csharp 

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

# Run the following command to get the values for this current stack’s configuration:
pulumi config
pulumi config --stack staging

# Let’s set the configuration for the staging stack
pulumi stack select staging
pulumi config set aws:region us-east-2
pulumi config set backendPort 3000
pulumi config set mongoPort 27017
pulumi config set mongoHost mongodb://mongo:27017
pulumi config set database cart
pulumi config set nodeEnvironment development
pulumi config set protocol http://

# You can access your outputs from the CLI by running
pulumi stack output [property-name]
pulumi stack output primaryStorageKey

# Destroy resources
pulumi destroy
pulumi destroy -y

# Delete this Stack's Resources
pulumi destroy -s psharpx/quickstart/dev

# Stack
pulumi stack init <stack_name|dev|uat|staging|prod>
pulumi stack ls
pulumi stack select <stack_name>
pulumi stack rm psharpx/quickstart/dev

# csharp commands
dotnet add package Pulumi.Docker


dotnet list SentimentAnalysis.csproj package
dotnet list package --format json