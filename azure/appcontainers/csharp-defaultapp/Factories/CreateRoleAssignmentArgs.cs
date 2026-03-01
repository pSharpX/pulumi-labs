using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace defaultapp.Factories;

public class CreateRoleAssignmentArgs: CreateResourceArgs
{
    public required Output<string> PrincipalId { get; set; }
    public PrincipalType PrincipalType { get; set; } = Pulumi.AzureNative.Authorization.PrincipalType.ServicePrincipal; 
    public Output<string>? Description { get; set; }
    public required Output<string> RoleDefinitionId { get; set; }
    public required Output<string> Scope { get; set; }
}