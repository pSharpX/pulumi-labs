using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace defaultapp.Factories;

public class CreateRoleAssignmentArgs: CreateResourceArgs
{
    public required Input<string> PrincipalId { get; set; }
    public PrincipalType PrincipalType { get; set; } = PrincipalType.ServicePrincipal; 
    public Input<string>? Description { get; set; }
    public required Input<string> RoleDefinitionId { get; set; }
    public required Input<string> Scope { get; set; }
}