using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sras.PublicCoreflow.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class PublicCoreflowDbContextFactory : IDesignTimeDbContextFactory<PublicCoreflowDbContext>
{
    public PublicCoreflowDbContext CreateDbContext(string[] args)
    {
        PublicCoreflowEfCoreEntityExtensionMappings.Configure();

        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<PublicCoreflowDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new PublicCoreflowDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Sras.PublicCoreflow.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
