using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Sras.PublicCoreflow.Web;

[Dependency(ReplaceServices = true)]
public class PublicCoreflowBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "PublicCoreflow";
}
