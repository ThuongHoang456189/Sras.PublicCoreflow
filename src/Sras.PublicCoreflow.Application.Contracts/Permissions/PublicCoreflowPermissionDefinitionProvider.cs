using Sras.PublicCoreflow.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Sras.PublicCoreflow.Permissions;

public class PublicCoreflowPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PublicCoreflowPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(PublicCoreflowPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PublicCoreflowResource>(name);
    }
}
