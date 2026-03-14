using Pulumi;
using Pulumi.AzureNative.Authorization;

namespace defaultapp.Factories;

public static class RoleAssignmentFactory
{
    public static RoleAssignment Create(CreateRoleAssignmentArgs args)
    {
        return new RoleAssignment($"OneBank_RoleAssignment_{args.Alias}", new RoleAssignmentArgs
        {
            RoleDefinitionId = args.RoleDefinitionId,
            PrincipalId = args.PrincipalId,
            Scope = args.Scope,
            PrincipalType = args.PrincipalType,
            RoleAssignmentName =  args.Name,
            Description = args.Description!,
        }, new CustomResourceOptions { Parent = args.Parent });
    }
}