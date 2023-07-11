using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.BlobStoring;
using Volo.Abp.BlobStoring.FileSystem;
using System;
using Volo.Abp.Timing;
using Volo.Abp.BackgroundWorkers;

namespace Sras.PublicCoreflow;

[DependsOn(
    typeof(PublicCoreflowDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(PublicCoreflowApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpBlobStoringModule),
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpBlobStoringFileSystemModule)
    )]
    public class PublicCoreflowApplicationModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<PublicCoreflowApplicationModule>();
            });

            Configure<AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseFileSystem(fileSystem =>
                    {
                        fileSystem.BasePath = ".";
                        fileSystem.AppendContainerNameToBasePath = true;
                    });
                });
            });

            Configure<AbpClockOptions>(options =>
            {
                options.Kind = DateTimeKind.Utc;
            });
        }
    }
