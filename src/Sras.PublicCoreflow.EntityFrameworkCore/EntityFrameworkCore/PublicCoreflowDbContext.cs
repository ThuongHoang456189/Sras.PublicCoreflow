using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class PublicCoreflowDbContext :
    AbpDbContext<PublicCoreflowDbContext>,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    #endregion

    #region Entities from the Sras Conference Management module
    public DbSet<PaperStatus> PaperStatuses { get; set; }
    public DbSet<Conference> Conferences { get; set; }
    public DbSet<ConferenceAccount> ConferenceAccounts { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<ConferenceRole> ConferenceRoles { get; set; }
    public DbSet<Incumbent> Incumbents { get; set; }
    #endregion

    public PublicCoreflowDbContext(DbContextOptions<PublicCoreflowDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(PublicCoreflowConsts.DbTablePrefix + "YourEntities", PublicCoreflowConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        builder.Entity<PaperStatus>(b =>
        {
            b.ToTable("PaperStatuses", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name)
            .HasMaxLength(PaperStatusConsts.MaxNameLength);
        });

        builder.Entity<Conference>(b =>
        {
            b.ToTable("Conferences", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.FullName)
            .HasMaxLength(ConferenceConsts.MaxFullnameLength);

            b.Property(x => x.ShortName)
            .HasMaxLength(ConferenceConsts.MaxShortNameLength);

            b.Property(x => x.City)
            .HasMaxLength(ConferenceConsts.MaxCityLength);

            b.Property(x => x.Country)
            .HasMaxLength(ConferenceConsts.MaxCountryLength);
        });

        builder.Entity<ConferenceAccount>(b =>
        {
            b.ToTable("ConferenceAccounts", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Track>(b =>
        {
            b.ToTable("Tracks", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(TrackConsts.MaxNameLength);

            b.Property(x => x.SubmissionInstruction)
            .HasMaxLength(TrackConsts.MaxSubmissionInstructionLength);
        });

        builder.Entity<ConferenceRole>(b =>
        {
            b.ToTable("ConferenceRoles", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(ConferenceRoleConsts.MaxNameLength);
        });

        builder.Entity<Incumbent>(b =>
        {
            b.ToTable("Incumbents", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });
    }
}
