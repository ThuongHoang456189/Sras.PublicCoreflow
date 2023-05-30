using Volo.Abp.Settings;

namespace Sras.PublicCoreflow.Settings;

public class PublicCoreflowSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(PublicCoreflowSettings.MySetting1));
    }
}
