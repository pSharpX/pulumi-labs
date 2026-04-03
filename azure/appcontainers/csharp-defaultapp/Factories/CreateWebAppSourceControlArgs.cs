namespace defaultapp.Factories;

public class CreateWebAppSourceControlArgs: CreateWebAppArgs
{
    public required string RepositoryUrl { get; set; }
    public string Branch { get; set; } = "main";
}