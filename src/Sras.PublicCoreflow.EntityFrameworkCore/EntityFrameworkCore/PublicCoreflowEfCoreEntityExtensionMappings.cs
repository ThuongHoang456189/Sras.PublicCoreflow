using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Sras.PublicCoreflow.EntityFrameworkCore;

public static class PublicCoreflowEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        PublicCoreflowGlobalFeatureConfigurator.Configure();
        PublicCoreflowModuleExtensionConfigurator.Configure();

        OneTimeRunner.Run(() =>
        {
            /* You can configure extra properties for the
             * entities defined in the modules used by your application.
             *
             * This class can be used to map these extra properties to table fields in the database.
             *
             * USE THIS CLASS ONLY TO CONFIGURE EF CORE RELATED MAPPING.
             * USE PublicCoreflowModuleExtensionConfigurator CLASS (in the Domain.Shared project)
             * FOR A HIGH LEVEL API TO DEFINE EXTRA PROPERTIES TO ENTITIES OF THE USED MODULES
             *
             * Example: Map a property to a table field:

                 ObjectExtensionManager.Instance
                     .MapEfCoreProperty<IdentityUser, string>(
                         "MyProperty",
                         (entityBuilder, propertyBuilder) =>
                         {
                             propertyBuilder.HasMaxLength(128);
                         }
                     );

             * See the documentation for more:
             * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
             */

            ObjectExtensionManager.Instance
            .MapEfCoreProperty<IdentityUser, string?>(
                AccountConsts.MiddleNamePropertyName,
                (_, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(AccountConsts.MaxMiddleNameLength);
                    propertyBuilder.HasDefaultValue(null);
                })
            .MapEfCoreProperty<IdentityUser, string?>(
                AccountConsts.OrganizationPropertyName,
                (_, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(AccountConsts.MaxOrganizationLength);
                    propertyBuilder.HasDefaultValue(null);
                })
            .MapEfCoreProperty<IdentityUser, string?>(
                AccountConsts.CountryPropertyName,
                (_, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(AccountConsts.MaxCountryLength);
                    propertyBuilder.HasDefaultValue(null);
                })
            .MapEfCoreProperty<IdentityUser, string?>(
                AccountConsts.DomainConflictsPropertyName,
                (_, propertyBuilder) =>
                {
                    propertyBuilder.HasMaxLength(AccountConsts.MaxDomainConflictsLength);
                    propertyBuilder.HasDefaultValue(null);
                });
        });
    }
}
