using Pulumi;
using Docker = Pulumi.Docker;

class ContainerLabelBuilder
{
    public ContainerLabelBuilder()
    {
        ContainerLabels = new InputList<Docker.Inputs.ContainerLabelArgs>
        {
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.project",
                Value = "OneBank Discovery"
            },
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.version",
                Value = "1.0"
            },
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.author.name",
                Value = "psharpx"
            },
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.author.email",
                Value = "crivera2093@gmail.com"
            },
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.author.url",
                Value = "https://github.com/pSharpX"
            },
            new Docker.Inputs.ContainerLabelArgs
            {
                Label = "com.trustbank.onebank.author.description",
                Value = "onebank Discovery PoC"
            }
        };
    }

    public InputList<Docker.Inputs.ContainerLabelArgs> ContainerLabels { get; set; }
}