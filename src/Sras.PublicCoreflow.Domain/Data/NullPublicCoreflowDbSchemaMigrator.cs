using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Sras.PublicCoreflow.Data;

/* This is used if database provider does't define
 * IPublicCoreflowDbSchemaMigrator implementation.
 */
public class NullPublicCoreflowDbSchemaMigrator : IPublicCoreflowDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
