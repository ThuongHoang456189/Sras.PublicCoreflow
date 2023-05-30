using Volo.Abp.Modularity;

namespace Sras.PublicCoreflow;

[DependsOn(
    typeof(PublicCoreflowApplicationModule),
    typeof(PublicCoreflowDomainTestModule)
    )]
public class PublicCoreflowApplicationTestModule : AbpModule
{

}
