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
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Outsider> Outsiders { get; set; }
    public DbSet<ActivityDeadline> ActivityDeadlines { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Reviewer> Reviewers { get; set; }
    public DbSet<ReviewerSubjectArea> ReviewerSubjectAreas { get; set; }
    public DbSet<Conflict> Conflicts { get; set; }
    public DbSet<ConflictCase> ConflictCases { get; set; }
    public DbSet<Email> Emails { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<InvitationClone> InvitationClones { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PlaceholderGroup> PlaceholderGroups { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionGroup> QuestionsGroups { get; set; }
    public DbSet<ReviewAssignment> ReviewAssignments { get; set; }
    public DbSet<SubjectArea> SubjectAreas { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<SubmissionClone> SubmissionClones { get; set; }
    public DbSet<SubmissionSubjectArea> SubmissionSubjectAreas { get; set; }
    public DbSet<SupportedPlaceholder> SupportedPlaceholders { get; set; }
    public DbSet<Revision> Revisions { get; set; }
    public DbSet<CameraReady> CameraReadies { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<RegistrationPaper> RegistrationPapers { get; set; }
    public DbSet<WebTemplate> WebTemplates { get; set; }
    public DbSet<Website> Websites { get; set; }
    public DbSet<Guideline> Guidelines { get; set; }
    public DbSet<SubmissionAttachment> SubmissionAttachments { get; set; }
    public DbSet<ResearcherProfile> ResearcherProfiles { get; set; }
    public virtual DbSet<SubmissionAggregationSP> SubmissionAggregationSPs { get; set; }
    public virtual DbSet<SubmissionSummarySPO> SubmissionSummarySPOs { get; set; }
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

            b.Property(x => x.TimeZone)
            .HasMaxLength(ConferenceConsts.MaxTimeZoneLength);
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

            b.Property(x => x.IsDecisionMaker)
            .HasDefaultValue(false);
        });

        builder.Entity<Participant>(b =>
        {
            b.ToTable("Participants", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Outsider>(b =>
        {
            b.ToTable("Outsiders", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Email)
            .HasMaxLength(OutsiderConsts.MaxEmailLength);

            b.Property(x => x.NamePrefix)
            .HasMaxLength(OutsiderConsts.MaxNamePrefixLength);

            b.Property(x => x.FirstName)
            .HasMaxLength(OutsiderConsts.MaxFirstNameLength);

            b.Property(x => x.MiddleName)
            .HasMaxLength(OutsiderConsts.MaxMiddleNameLength);

            b.Property(x => x.LastName)
            .HasMaxLength(OutsiderConsts.MaxLastNameLength);

            b.Property(x => x.Organization)
            .HasMaxLength(OutsiderConsts.MaxOrganizationLength);

            b.Property(x => x.Country)
            .HasMaxLength(OutsiderConsts.MaxCountryLength);
        });

        builder.Entity<ActivityDeadline>(b =>
        {
            b.ToTable("ActivityDeadlines", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Phase)
            .HasMaxLength(ActivityDeadlineConsts.MaxPhaseLength);

            b.Property(x => x.Name)
            .HasMaxLength(ActivityDeadlineConsts.MaxNameLength);

            b.Property(x => x.GuidelineGroup)
            .HasMaxLength(ActivityDeadlineConsts.MaxGuidelineGroupLength);
        });

        builder.Entity<Author>(b =>
        {
            b.ToTable("Authors", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Reviewer>(b =>
        {
            b.ToTable("Reviewers", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasOne<Incumbent>(i => i.Incumbent)
            .WithMany(s => s.Reviewers)
            .HasForeignKey(i => i.Id)
            .IsRequired(true);
        });

        builder.Entity<ReviewerSubjectArea>(b =>
        {
            b.ToTable("ReviewerSubjectAreas", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<Conflict>(b =>
        {
            b.ToTable("Conflicts", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<ConflictCase>(b =>
        {
            b.ToTable("ConflictCases", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(128);
        });

        builder.Entity<Email>(b =>
        {
            b.ToTable("Emails", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Subject)
            .HasMaxLength(2048);
        });

        builder.Entity<EmailTemplate>(b =>
        {
            b.ToTable("EmailTemplates", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(EmailTemplateConsts.MaxNameLength);

            b.Property(x => x.Subject)
            .HasMaxLength(2048);
        });

        builder.Entity<Invitation>(b =>
        {
            b.ToTable("Invitations", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Comment)
            .HasMaxLength(2048);
        });

        builder.Entity<InvitationClone>(b =>
        {
            b.ToTable("InvitationClones", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.InviteeUrl)
            .HasMaxLength(2048);

            b.Property(x => x.InviteeAcceptUrl)
            .HasMaxLength(2048);

            b.Property(x => x.InviteeDeclineUrl)
            .HasMaxLength(2048);
        });

        builder.Entity<Order>(b =>
        {
            b.ToTable("Orders", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Currency)
            .HasMaxLength(64);
        });

        builder.Entity<Payment>(b =>
        {
            b.ToTable("Payments", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Status)
            .HasMaxLength(64);

            b.Property(x => x.PaymentProofRootFilePath)
            .HasMaxLength(1024);
        });

        builder.Entity<PlaceholderGroup>(b =>
        {
            b.ToTable("PlaceholderGroups", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(128);
        });

        builder.Entity<Question>(b =>
        {
            b.ToTable("Questions", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
            .HasMaxLength(2048);

            b.Property(x => x.Text)
            .HasMaxLength(2048);

            b.Property(x => x.Type)
            .HasMaxLength(64);

            b.Property(x => x.TypeName)
            .HasMaxLength(64);
        });

        builder.Entity<QuestionGroup>(b =>
        {
            b.ToTable("QuestionGroups", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(256);
        });

        builder.Entity<ReviewAssignment>(b =>
        {
            b.ToTable("ReviewAssignments", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<SubjectArea>(b =>
        {
            b.ToTable("SubjectAreas", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(SubjectAreaConsts.MaxNameLength);
        });

        builder.Entity<Submission>(b =>
        {
            b.ToTable("Submissions", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
            .HasMaxLength(SubmissionConsts.MaxTitleLength);

            b.Property(x => x.Abstract)
            .HasMaxLength(SubmissionConsts.MaxAbstractLength);

            b.Property(x => x.RootFilePath)
            .HasMaxLength(SubmissionConsts.MaxRootFilePathLength);

            b.Property(x => x.IsRequestedForPresentation)
            .HasDefaultValue(false);

            b.Property(x => x.DomainConflicts)
            .HasMaxLength(SubmissionConsts.MaxDomainConflictsLength);

            b.HasOne<Incumbent>(i => i.CreatedIncumbent)
            .WithMany(s => s.CreationSubmissions)
            .HasForeignKey(i => i.Id)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.SetNull);

            b.HasOne<Incumbent>(i => i.LastModifiedIncumbent)
            .WithMany(s => s.ModificationSubmissions)
            .HasForeignKey(i => i.Id)
            .IsRequired(true)
            .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Revision>(b =>
        {
            b.ToTable("Revisions", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RootFilePath)
            .HasMaxLength(RevisionConsts.MaxRootFilePathLength);
        });

        builder.Entity<SubmissionClone>(b =>
        {
            b.ToTable("SubmissionClones", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.IsLast)
            .HasDefaultValue(false);
        });

        builder.Entity<SubmissionSubjectArea>(b =>
        {
            b.ToTable("SubmissionSubjectAreas", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<SupportedPlaceholder>(b =>
        {
            b.ToTable("SupportedPlaceholders", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Encode)
            .HasMaxLength(128);

            b.Property(x => x.Description)
            .HasMaxLength(1024);
        });

        builder.Entity<CameraReady>(b =>
        {
            b.ToTable("CameraReadies", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RootCameraReadyFilePath)
            .HasMaxLength(CameraReadyConsts.MaxFilePathLength);

            b.Property(x => x.CopyRightFilePath)
            .HasMaxLength(CameraReadyConsts.MaxFilePathLength);
        });

        builder.Entity<Registration>(b =>
        {
            b.ToTable("Registrations", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<RegistrationPaper>(b =>
        {
            b.ToTable("RegistrationPapers", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RootPresentationFilePath)
            .HasMaxLength(RegistrationPaperConsts.MaxRootPresentationFilePathLength);
        });

        builder.Entity<WebTemplate>(b =>
        {
            b.ToTable("WebTemplates", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(128);

            b.Property(x => x.Description)
            .HasMaxLength(1024);

            b.Property(x => x.RootFilePath)
            .HasMaxLength(PublicCoreflowConsts.MaxRootFilePathLength);
        });

        builder.Entity<Website>(b =>
        {
            b.ToTable("Websites", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RootFilePath)
            .HasMaxLength(PublicCoreflowConsts.MaxRootFilePathLength);

            b.Property(x => x.TempFilePath)
            .HasMaxLength(PublicCoreflowConsts.MaxRootFilePathLength);
        });

        builder.Entity<Guideline>(b =>
        {
            b.ToTable("Guidelines", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(GuidelineConsts.MaxNameLength);

            b.Property(x => x.Description)
            .HasMaxLength(GuidelineConsts.MaxDescriptionLength);

            b.Property(x => x.GuidelineGroup)
            .HasMaxLength(GuidelineConsts.MaxGuidelineGroupLength);

            b.Property(x => x.Route)
            .HasMaxLength(GuidelineConsts.MaxRouteLength);
        });

        builder.Entity<SubmissionAttachment>(b =>
        {
            b.ToTable("SubmissionAttachments", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.RootSupplementaryMaterialFilePath)
            .HasMaxLength(SubmissionAttachmentConsts.MaxRootFilePathLength);

            b.Property(x => x.RootPresentationFilePath)
            .HasMaxLength(SubmissionAttachmentConsts.MaxRootFilePathLength);
        });

        builder.Entity<ResearcherProfile>(b =>
        {
            b.ToTable("ResearcherProfiles", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.PublishName)
            .HasMaxLength(256);

            b.Property(x => x.PrimaryEmail)
            .HasMaxLength(256);

            b.Property(x => x.Introduction)
            .HasMaxLength(2048);

            b.Property(x => x.Gender)
            .HasMaxLength(64);

            b.Property(x => x.CurrentResearchScientistTitle)
            .HasMaxLength(512);

            b.Property(x => x.CurrentAdministrationPosition)
            .HasMaxLength(512);

            b.Property(x => x.CurrentAcademicFunction)
            .HasMaxLength(512);

            b.Property(x => x.CurrentDegree)
            .HasMaxLength(512);

            b.Property(x => x.HomeAddress)
            .HasMaxLength(1024);

            b.Property(x => x.PhoneNumber)
            .HasMaxLength(16);

            b.Property(x => x.MobilePhoneNumber)
            .HasMaxLength(16);

            b.Property(x => x.Fax)
            .HasMaxLength(32);
        });

        builder.Entity<SubmissionAggregationSP>(b =>
        {
            b.HasNoKey();
        });

        builder.Entity<SubmissionSummarySPO>(b =>
        {
            b.HasNoKey();
        });
    }
}
