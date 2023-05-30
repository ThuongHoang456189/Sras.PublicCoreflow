using Sras.PublicCoreflow.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Sras.PublicCoreflow.Web.Pages;

public abstract class PublicCoreflowPageModel : AbpPageModel
{
    protected PublicCoreflowPageModel()
    {
        LocalizationResourceType = typeof(PublicCoreflowResource);
    }
}
