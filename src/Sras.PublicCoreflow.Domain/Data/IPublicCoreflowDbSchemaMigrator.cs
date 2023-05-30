using System.Threading.Tasks;

namespace Sras.PublicCoreflow.Data;

public interface IPublicCoreflowDbSchemaMigrator
{
    Task MigrateAsync();
}
