using Microsoft.AspNetCore.Identity;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.Data
{
    public class ConferenceManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;

        private readonly IRepository<PaperStatus, Guid> _paperStatusRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;
        private readonly IRepository<Participant, Guid> _participantRepository;
        private readonly IRepository<PlaceholderGroup, Guid> _placeholderGroupRepository;
        private readonly IRepository<SupportedPlaceholder, Guid> _supportedPlaceholderRepository;
        private readonly IRepository<ConflictCase, Guid> _conflictCaseRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        private Guid _sandraId;
        private Guid _sergeyId;
        private Guid _wellyId;
        private Guid _alessandroId;
        private Guid _markId;
        private Guid _tonyId;
        private Guid _davidGrausId;
        private Guid _davidRossId;
        private Guid _tomId;
        private Guid _shreeId;

        private IdentityUser _sandra;
        private IdentityUser _sergey;
        private IdentityUser _welly;
        private IdentityUser _alessandro;
        private IdentityUser _mark;
        private IdentityUser _tony;
        private IdentityUser _davidGraus;
        private IdentityUser _davidRoss;
        private IdentityUser _tom;
        private IdentityUser _shree;

        private Guid _conferencePlaceholderGroup;
        private Guid _submissionPlaceholderGroup;
        private Guid _senderPlaceholderGroup;
        private Guid _recipientPlaceholderGroup;

        public ConferenceManagementDataSeedContributor(
            IGuidGenerator guidGenerator,
            IRepository<PaperStatus, Guid> paperStatusRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository,
            IRepository<Participant, Guid> participantRepository,
            IRepository<PlaceholderGroup, Guid> placeholderGroupRepository,
            IRepository<SupportedPlaceholder, Guid> supportedPlaceholderRepository,
            IRepository<ConflictCase, Guid> conflictCaseRepository,
            IdentityUserManager identityUserManager,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _guidGenerator = guidGenerator;
            _paperStatusRepository = paperStatusRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
            _participantRepository = participantRepository;
            _placeholderGroupRepository = placeholderGroupRepository;
            _supportedPlaceholderRepository = supportedPlaceholderRepository;
            _conflictCaseRepository = conflictCaseRepository;

            _identityUserManager = identityUserManager;
            _userRepository = userRepository;
        }

        private async Task CreateParticipantsAsync()
        {
            if(await _participantRepository.GetCountAsync() > 0)
            {
                return;
            }

            var participants = new List<Participant>
            {
                new Participant(_guidGenerator.Create(), _sandraId, null),
                new Participant(_guidGenerator.Create(), _sergeyId, null),
                new Participant(_guidGenerator.Create(), _wellyId, null),
                new Participant(_guidGenerator.Create(), _alessandroId, null),
                new Participant(_guidGenerator.Create(), _markId, null),
                new Participant(_guidGenerator.Create(), _tonyId, null),
                new Participant(_guidGenerator.Create(), _davidGrausId, null),
                new Participant(_guidGenerator.Create(), _davidRossId, null),
                new Participant(_guidGenerator.Create(), _tomId, null),
                new Participant(_guidGenerator.Create(), _shreeId, null),
            };

            await _participantRepository.InsertManyAsync(participants);
        }

        private async Task CreateSampleUsersAsync()
        {
            _sandra = new IdentityUser(_sandraId = _guidGenerator.Create(), "SandraWolf", "sandra_wolf@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "Hoa Lac Campus, FPT University")
                .SetProperty(AccountConsts.CountryPropertyName, "India");
            _sandra.Name = "Sandra";
            _sandra.Surname = "Wolf";

            _sergey = new IdentityUser(_sergeyId = _guidGenerator.Create(), "SergeyPolgul", "sergey_polgul@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "HCM Campus, FPT University")
                .SetProperty(AccountConsts.CountryPropertyName, "Laos");
            _sergey.Name = "Sergey";
            _sergey.Surname = "Polgul";

            _welly = new IdentityUser(_wellyId = _guidGenerator.Create(), "WellyTambunan", "welly_tambunan@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "Ton Duc Thang University")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _welly.Name = "Welly";
            _welly.Surname = "Tambunan";

            _alessandro = new IdentityUser(_alessandroId = _guidGenerator.Create(), "AlessandroMuci", "alessandro_muci@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "Hutech University")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _alessandro.Name = "Alessandro";
            _alessandro.Surname = "Muci";

            _mark = new IdentityUser(_markId = _guidGenerator.Create(), "MarkGodfrey", "mark_godfrey@gmail.com")
                .SetProperty(AccountConsts.CountryPropertyName, "India");
            _mark.Name = "Mark";
            _mark.Surname = "Godfrey";

            _tony = new IdentityUser(_tonyId = _guidGenerator.Create(), "TonyBurton", "tony_burton@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "HCM Campus, FPT University")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _tony.Name = "Tony";
            _tony.Surname = "Burton";

            _davidGraus = new IdentityUser(_davidGrausId = _guidGenerator.Create(), "DavidGraus", "david_graus@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "HCM Campus, FPT University")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _davidGraus.Name = "David";
            _davidGraus.Surname = "Graus";

            _davidRoss = new IdentityUser(_davidRossId = _guidGenerator.Create(), "DavidRoss", "david_ross@gmail.com")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _davidRoss.Name = "David";
            _davidRoss.Surname = "Ross";

            _tom = new IdentityUser(_tomId = _guidGenerator.Create(), "TomLidy", "tom_lidy@gmail.com")
                .SetProperty(AccountConsts.CountryPropertyName, "India");
            _tom.Name = "Tom";
            _tom.Surname = "Lidy";

            _shree = new IdentityUser(_shreeId = _guidGenerator.Create(), "ShreePatel", "shree_patel@gmail.com")
                .SetProperty(AccountConsts.OrganizationPropertyName, "HCM Campus, FPT University")
                .SetProperty(AccountConsts.CountryPropertyName, "Vietnam");
            _shree.Name = "Shree";
            _shree.Surname = "Patel";

            var users = new List<IdentityUser>
            {
                _sandra,
                _sergey,
                _welly,
                _alessandro,
                _mark,
                _tony,
                _davidGraus,
                _davidRoss,
                _tom,
                _shree,
            };

            foreach (var user in users)
            {
                (await _identityUserManager.CreateAsync(user, "Abc#123")).CheckErrors();
            }
        }

        private async Task CreatePaperStatusesAsync()
        {
            if (await _paperStatusRepository.GetCountAsync() > 0)
            {
                return;
            }

            var paperStatuses = new List<PaperStatus>
            {
                new PaperStatus(_guidGenerator.Create(),"Awaiting Decision", null, false, true),
                new PaperStatus(_guidGenerator.Create(),"Withdrawn", null, false, true),
                new PaperStatus(_guidGenerator.Create(),"Desk Reject", null, false, true),
                new PaperStatus(_guidGenerator.Create(),"Accept", null, false, true),
                new PaperStatus(_guidGenerator.Create(),"Revision", null, false, true),
                new PaperStatus(_guidGenerator.Create(),"Reject", null, false, true)
            };

            await _paperStatusRepository.InsertManyAsync(paperStatuses, autoSave: true);
        }

        private async Task CreateConferenceRolesAsync()
        {
            if (await _conferenceRoleRepository.GetCountAsync() > 0)
            {
                return;
            }

            var conferenceRoles = new List<ConferenceRole>
            {
                new ConferenceRole(_guidGenerator.Create(), "Chair", true, 1),
                new ConferenceRole(_guidGenerator.Create(), "Track Chair", true, 2),
                new ConferenceRole(_guidGenerator.Create(), "Reviewer", true, 3),
                new ConferenceRole(_guidGenerator.Create(), "Author", false, 4),
            };

            await _conferenceRoleRepository.InsertManyAsync(conferenceRoles, autoSave: true);
        }

        private async Task CreatePlaceholderGroupAsync()
        {
            if (await _placeholderGroupRepository.GetCountAsync() > 0)
            {
                return;
            }

            var placeholderGroups = new List<PlaceholderGroup>
            {
                new PlaceholderGroup(_conferencePlaceholderGroup = _guidGenerator.Create(), "Conference"),
                new PlaceholderGroup(_submissionPlaceholderGroup = _guidGenerator.Create(), "Submission"),
                new PlaceholderGroup(_senderPlaceholderGroup = _guidGenerator.Create(), "Sender"),
                new PlaceholderGroup(_recipientPlaceholderGroup = _guidGenerator.Create(), "Recipient"),
            };

            await _placeholderGroupRepository.InsertManyAsync(placeholderGroups, autoSave: true);
        }

        private async Task CreateSupportedPlaceholderAsync()
        {
            if (await _supportedPlaceholderRepository.GetCountAsync() > 0)
            {
                return;
            }

            var supportedPlaceholders = new List<SupportedPlaceholder>
            {
                new SupportedPlaceholder(_guidGenerator.Create(), "{Conference.Name}", "Name of the conference", _conferencePlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Conference.StartDate}", "Start date of the conference", _conferencePlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Conference.EndDate}", "End date of the conference", _conferencePlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Conference.City}", "City of the conference", _conferencePlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Conference.Country}", "Country of the conference", _conferencePlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.Id}", "Id of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.Title}", "Title of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.Abstract}", "Abstract of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.StatusName}", "Status of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.TrackName}", "Track of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.CreateDate}", "Create date of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.UpdateDate}", "Last update date of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Submission.PrimarySubjectArea.Name}", "Name of primary subject area of submission", _submissionPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Sender.Name}", "Sender full name", _senderPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Sender.FirstName}", "Sender first name", _senderPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Sender.LastName}", "Sender last name", _senderPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Sender.Email}", "Sender email", _senderPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Sender.Organization}", "Sender organization", _senderPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Recipient.Name}", "Recipient full name", _recipientPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Recipient.FirstName}", "Recipient first name", _recipientPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Recipient.LastName}", "Recipient last name", _recipientPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Recipient.Email}", "Recipient email", _recipientPlaceholderGroup),
                new SupportedPlaceholder(_guidGenerator.Create(), "{Recipient.Organization}", "Recipient organization", _recipientPlaceholderGroup)
            };

            await _supportedPlaceholderRepository.InsertManyAsync(supportedPlaceholders, autoSave: true);
        }


        private async Task CreateConflictCaseAsync()
        {
            if (await _conflictCaseRepository.GetCountAsync() > 0)
            {
                return;
            }

            var conflictCases = new List<ConflictCase>
            {
                new ConflictCase(_guidGenerator.Create(), "Is a author/co-author", true, true, null),
                new ConflictCase(_guidGenerator.Create(), "Is/Was a colleague (in last 2 years)", true, true, null),
                new ConflictCase(_guidGenerator.Create(), "Is/Was a collaborator (in last 2 years)", true, true, null),
                new ConflictCase(_guidGenerator.Create(), "Is/Was a Primary Thesis Advisor at any time", true, true, null),
                new ConflictCase(_guidGenerator.Create(), "Is a relative or a friend", true, true, null),
                new ConflictCase(_guidGenerator.Create(), "Has (a) domain conflicts", false, true, null),
            };

            await _conflictCaseRepository
                .InsertManyAsync(conflictCases, autoSave: true);
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _userRepository.GetCountAsync() <= 1)
            {
                await CreateSampleUsersAsync();
                await CreateParticipantsAsync();
            }

            await CreatePaperStatusesAsync();
            await CreateConferenceRolesAsync();
            await CreatePlaceholderGroupAsync();
            await CreateSupportedPlaceholderAsync();
            await CreateConflictCaseAsync();
        }
    }
}
