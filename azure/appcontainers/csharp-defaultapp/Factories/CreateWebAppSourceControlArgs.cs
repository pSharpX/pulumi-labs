namespace defaultapp.Factories;

public class CreateWebAppSourceControlArgs: CreateResourceArgs
{
    public required string Alias { get; set; }
    public required string RepositoryUrl { get; set; }
    public string Branch { get; set; } = "main";
}