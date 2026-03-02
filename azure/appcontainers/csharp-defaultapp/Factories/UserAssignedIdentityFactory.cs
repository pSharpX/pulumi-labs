using Pulumi;
using Pulumi.AzureNative.ManagedIdentity;

namespace defaultapp.Factories;

public static class UserAssignedIdentityFactory
{
    public static UserAssignedIdentity Create(CreateUserAssignedIdentityArgs args)
    {
        return new UserAssignedIdentity("OneBank_UserAssignedIdentity", new UserAssignedIdentityArgs
        {
            Location = args.Location,
            ResourceGroupName =  args.ResourceGroupName,
            ResourceName =args.Name,
            Tags =  args.Tags!
        }, new CustomResourceOptions{ Parent = args.Parent });
    }
}