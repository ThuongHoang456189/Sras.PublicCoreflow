using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sras.PublicCoreflow.Data;
using Volo.Abp.DependencyInjection;

namespace Sras.PublicCoreflow.EntityFrameworkCore;

public class EntityFrameworkCorePublicCoreflowDbSchemaMigrator
    : IPublicCoreflowDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorePublicCoreflowDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the PublicCoreflowDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PublicCoreflowDbContext>()
            .Database
            .MigrateAsync();
    }
}
