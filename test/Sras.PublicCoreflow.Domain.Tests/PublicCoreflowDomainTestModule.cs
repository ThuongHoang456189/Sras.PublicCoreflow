using Sras.PublicCoreflow.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Sras.PublicCoreflow;

[DependsOn(
    typeof(PublicCoreflowEntityFrameworkCoreTestModule)
    )]
public class PublicCoreflowDomainTestModule : AbpModule
{

}
