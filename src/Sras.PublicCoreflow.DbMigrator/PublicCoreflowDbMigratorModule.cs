using Sras.PublicCoreflow.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Sras.PublicCoreflow.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PublicCoreflowEntityFrameworkCoreModule),
    typeof(PublicCoreflowApplicationContractsModule)
    )]
public class PublicCoreflowDbMigratorModule : AbpModule
{

}
