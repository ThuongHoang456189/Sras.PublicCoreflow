using Sras.PublicCoreflow.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class PublicCoreflowController : AbpControllerBase
{
    protected PublicCoreflowController()
    {
        LocalizationResource = typeof(PublicCoreflowResource);
    }
}
