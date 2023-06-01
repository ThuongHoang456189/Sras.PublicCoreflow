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
    public DbSet<ConferenceReviewer> ConferenceReviewers { get; set; }
    public DbSet<ConferenceReviewerSubjectArea> ConferenceReviewerSubjectAreas { get; set; }
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
    public DbSet<QuestionGroupTrack> QuestionGroupTracks { get; set; }
    public DbSet<ReviewAssignment> ReviewAssignments { get; set; }
    public DbSet<SubjectArea> SubjectAreas { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<SubmissionClone> SubmissionClones { get; set; }
    public DbSet<SubmissionSubjectArea> SubmissionSubjectAreas { get; set; }
    public DbSet<SupportedPlaceholder> SupportedPlaceholders { get; set; }
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

            b.Property(x => x.FirstName)
            .HasMaxLength(OutsiderConsts.MaxFirstNameLength);

            b.Property(x => x.MiddleName)
            .HasMaxLength(OutsiderConsts.MaxMiddleNameLength);

            b.Property(x => x.LastName)
            .HasMaxLength(OutsiderConsts.MaxLastNameLength);

            b.Property(x => x.Organization)
            .HasMaxLength(OutsiderConsts.MaxOrganizationLength);
        });

        builder.Entity<ActivityDeadline>(b =>
        {
            b.ToTable("ActivityDeadlines", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(64);
        });

        builder.Entity<Author>(b =>
        {
            b.ToTable("Authors", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.ParticipantId, x.SubmissionId });
        });

        builder.Entity<ConferenceReviewer>(b =>
        {
            b.ToTable("ConferenceReviewers", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<ConferenceReviewerSubjectArea>(b =>
        {
            b.ToTable("ConferenceReviewerSubjectAreas", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.ConferenceReviewerId, x.SubjectAreaId });
        });

        builder.Entity<Conflict>(b =>
        {
            b.ToTable("Conflicts", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.SubmissionId, x.IncumbentId, x.ConflictCaseId });
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
        });

        builder.Entity<QuestionGroup>(b =>
        {
            b.ToTable("QuestionGroups", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Name)
            .HasMaxLength(64);
        });

        builder.Entity<QuestionGroupTrack>(b =>
        {
            b.ToTable("QuestionGroupTracks", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
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
            .HasMaxLength(512);
        });

        builder.Entity<Submission>(b =>
        {
            b.ToTable("Submissions", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.Property(x => x.Title)
            .HasMaxLength(1024);

            b.Property(x => x.Abstract)
            .HasMaxLength(2048);

            b.Property(x => x.RootFilePath)
            .HasMaxLength(1024);

            b.Property(x => x.DomainConflicts)
            .HasMaxLength(1024);

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

        builder.Entity<SubmissionClone>(b =>
        {
            b.ToTable("SubmissionClones", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();
        });

        builder.Entity<SubmissionSubjectArea>(b =>
        {
            b.ToTable("SubmissionSubjectAreas", PublicCoreflowConsts.DbSchema);
            b.ConfigureByConvention();

            b.HasKey(x => new { x.SubmissionId, x.SubjectAreaId });
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
    }
}
