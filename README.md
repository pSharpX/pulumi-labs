# pulumi-labs
handy set of recipes for provisioning infra using Pulumi

## Pulumi CLI
```bash
pulumi --help
pulumi <command> --help
pulumi version
```

## Login/Logout to Pulumi Cloud
```bash
pulumi login
pulumi logout
```

## Pulumi Template Generator
Options can be one: TypeScript, JavaScript, Python, Go, C#, Java, or YAML:

Examples:
```bash
pulumi new
pulumi new csharp
pulumi new aws-csharp
pulumi new azure-csharp
pulumi new gcp-csharp
pulumi new visualbasic
pulumi new java
pulumi stack
```

This csharp template is cloud agnostic, and you will need to install NuGet packages for the cloud provider of your choice
```bash
pulumi new csharp 
```

## Create or Update resources in a stack.
```bash
pulumi up
# Automatically approve and perform the update after previewing it
pulumi up -y
```
```bash
pulumi up --diff
pulumi preview
```

## --skip-preview
To do not calculate a preview before performing the update

```bash
pulumi up -f 
```

## --stack_name (string)
The name of the stack to operate on. Defaults to the current stack

```bash
pulumi up -s <stack_name>
```

## Configuations 
Run the following command to get the values for the current stack’s configuration:

```bash
pulumi config
pulumi config --stack staging
```

Let’s `Set/Get` the configuration for the staging stack

```bash
pulumi stack select staging
pulumi config set aws:region us-east-2
pulumi config set backendPort 3000
pulumi config set mongoPort 27017
pulumi config set mongoHost mongodb://mongo:27017
pulumi config set database cart
pulumi config set nodeEnvironment development
pulumi config set protocol http://
cat ../../../ssh/ec2-keys | pulumi config set --secret privateKey

pulumi config get database
pulumi config get nodeEnvironment
pulumi config get publicKey
```

You can access your outputs from the CLI by running

```bash
pulumi stack output [property-name]
pulumi stack output primaryStorageKey
pulumi stack output instanceIp
pulumi stack output instanceHost
pulumi stack output password --show-secrets
```

## Destroy resources
```bash
pulumi destroy
pulumi destroy -y
```

Delete specific Stack's Resources

```bash
pulumi destroy -s psharpx/quickstart/dev
```

## Stack
```bash
pulumi stack init <stack_name|dev|uat|staging|prod>
pulumi stack ls
pulumi stack select <stack_name>
pulumi stack rm psharpx/quickstart/dev
```

## Pulumi ESC Environments
To work with pulumi environment you can use ESC (Environment, Secrets and Configuration) CLI or using pulumi env command (https://www.pulumi.com/docs/install/esc/)

```bash
pulumi env --help
pulumi env init psharpx/dev
pulumi env rm psharpx/dev

pulumi env open psharpx/dev
## Default output format is json
pulumi env open psharpx/dev --format 'yaml'
pulumi env open psharpx/dev --format 'dotenv'

## Getting and setting environment values
pulumi env set psharpx/dev greeting hello-world
pulumi env get psharpx/dev foo
pulumi env get psharpx/dev
pulumi env set psharpx/dev mysecret iam_a_secret --secret
pulumi env set psharpx/dev 'data.active' true
pulumi env set psharpx/dev 'data.nums[0]' 1
pulumi env get psharpx/dev 'data.nums[1]'

pulumi env set psharpx/dev 'aws.accessKey' value --secret
pulumi env set psharpx/dev 'aws.secretKey' value --secret

pulumi env set psharpx/dev 'environmentVariables.AWS_ACCESS_KEY_ID' '${aws.accessKey}'
pulumi env set psharpx/dev 'environmentVariables.AWS_SECRET_ACCESS_KEY' '${aws.secretKey}'

pulumi env set psharpx/dev 'pulumiConfig.aws:accessKey' '${aws.accessKey}'
pulumi env set psharpx/dev 'pulumiConfig.aws:secretKey' '${aws.secretKey}'
```

###  Running commands with environment variables
To do this, use esc run `<environment-name> <command>`:

```bash
pulumi env run psharpx/dev aws s3 ls
## If you need to pass one or more flags to the command, prefix the command with --:
pulumi env run psharpx/dev -- aws s3 ls s3://my-s3-bucket --recursive --summarize
```

### Interpolating values
```bash
pulumi env set psharpx/dev salutation Hello
pulumi env set psharpx/dev name World
pulumi env set psharpx/dev greeting '${salutation}, ${name}'
pulumi env get psharpx/dev greeting
```

# csharp commands
dotnet add package Pulumi.Docker


dotnet list SentimentAnalysis.csproj package
dotnet list package --format json

# Generating a private RSA key using OpenSSL
## Generate an RSA private key, of size 2048, and output it to a file named key.pem:
openssl genrsa -out ./ssh/ec2-keypair.pem 2048

## Extract the public key from the key pair, which can be used in a certificate:
openssl rsa -in ./ssh/ec2-keypair.pem -outform PEM -pubout -out ./ssh/ec2-keypair-pub.pem

# Next, generate an OpenSSH keypair for use with your server - as per the AWS Requirements
## when PEM format is set the header is created as  RSA PRIVATE KEY

## RSA PRIVATE KEY
ssh-keygen -t rsa -f ./ssh/ec2-keys -m PEM 
## OPENSSH PRIVATE KEY
ssh-keygen -t rsa -f ./ssh/ec2-keys-2

## AWS
Retrieving AWS AMI information using Pulumi environment command:

```bash
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners 099720109477 --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=ubuntu*"
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners 099720109477 --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=ubuntu/images/hvm-ssd/ubuntu-jammy-22.04-amd64-server-*" --query 'Images[?CreationDate>`2022-06-01`] | sort_by(@, &CreationDate)'
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=platform,Values=windows"
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=platform,Values=windows" "Name=name,Values=Windows_Server-2022-English-*-Base*" --query 'Images[?CreationDate>`2022-06-01`] | sort_by(@, &CreationDate)'
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=amzn*"
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=amzn2-ami-hvm*-x86_64-gp2" --query 'Images[?CreationDate>`2022-06-01`] | sort_by(@, &CreationDate)'
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=al2023-ami-minimal-2023*-x86_64" --query 'Images[?CreationDate>`2022-06-01`] | sort_by(@, &CreationDate)'
pulumi env run psharpx/dev -- aws ec2 describe-images --region us-east-1 --owners amazon --filters "Name=virtualization-type,Values=hvm" "Name=architecture,Values=x86_64" "Name=name,Values=RHEL_8.5-x86_64-*" --query 'Images[?CreationDate>`2022-06-01`] | sort_by(@, &CreationDate)'
```