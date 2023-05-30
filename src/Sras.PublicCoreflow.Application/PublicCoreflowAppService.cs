using System;
using System.Collections.Generic;
using System.Text;
using Sras.PublicCoreflow.Localization;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow;

/* Inherit your application services from this class.
 */
public abstract class PublicCoreflowAppService : ApplicationService
{
    protected PublicCoreflowAppService()
    {
        LocalizationResource = typeof(PublicCoreflowResource);
    }
}
