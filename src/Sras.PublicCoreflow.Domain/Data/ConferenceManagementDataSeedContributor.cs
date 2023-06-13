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
        private readonly IRepository<Conference, Guid> _conferenceRepository;
        private readonly IRepository<Track, Guid> _trackConferenceRepository;
        private readonly IRepository<SubjectArea, Guid> _subjectAreaRepository;
        private readonly IRepository<EmailTemplate, Guid> _emailTemplateRepository;
        private readonly IRepository<Submission, Guid> _submissionRepository;

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

        private Guid _conference1stSciId;
        private Guid _conference2ndSciId;
        private Guid _conference3rdSciId;
        private Guid _conferenceSingleTrackId;

        private Guid _track1stSci1Id;
        private Guid _track1stSci2Id;
        private Guid _track2ndSci1Id;
        private Guid _track2ndSci2Id;
        private Guid _track2ndSci3Id;
        private Guid _track3rdSci1Id;
        private Guid _track3rdSci2Id;
        private Guid _track3rdSci3Id;
        private Guid _trackSingleId;

        private Guid _paperStatusAwaitingDecision;
        private Guid _paperStatusWithdrawn;
        private Guid _paperStatusDeskReject;
        private Guid _paperStatusAccept;
        private Guid _paperStatusRevision;
        private Guid _paperStatusReject;

        public ConferenceManagementDataSeedContributor(
            IGuidGenerator guidGenerator,
            IRepository<PaperStatus, Guid> paperStatusRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository,
            IRepository<Participant, Guid> participantRepository,
            IRepository<PlaceholderGroup, Guid> placeholderGroupRepository,
            IRepository<SupportedPlaceholder, Guid> supportedPlaceholderRepository,
            IRepository<ConflictCase, Guid> conflictCaseRepository,
            IdentityUserManager identityUserManager,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<Conference, Guid> conferenceRepository,
            IRepository<Track, Guid> trackConferenceRepository,
            IRepository<SubjectArea, Guid> subjectAreaRepository,
            IRepository<EmailTemplate, Guid> emailTemplateRepository,
            IRepository<Submission, Guid> submissionRepository)
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
            _conferenceRepository = conferenceRepository;
            _trackConferenceRepository = trackConferenceRepository;
            _subjectAreaRepository = subjectAreaRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _submissionRepository = submissionRepository;
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
                new PaperStatus(_paperStatusAwaitingDecision = _guidGenerator.Create(),"Awaiting Decision", null, false, true),
                new PaperStatus(_paperStatusWithdrawn = _guidGenerator.Create(),"Withdrawn", null, false, true),
                new PaperStatus(_paperStatusDeskReject = _guidGenerator.Create(),"Desk Reject", null, false, true),
                new PaperStatus(_paperStatusAccept = _guidGenerator.Create(),"Accept", null, false, true),
                new PaperStatus(_paperStatusRevision = _guidGenerator.Create(),"Revision", null, false, true),
                new PaperStatus(_paperStatusReject = _guidGenerator.Create(),"Reject", null, false, true)
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

        private async Task CreateConferenceAsync()
        {/* Add data for Conference table */
            var conferences = new List<Conference>
            {
                new Conference(_conference1stSciId = _guidGenerator.Create(), "First Science Conference", "1stSci", "HoChiMinh", "VietNam", new DateTime(2022, 04, 23), new DateTime(2022, 08, 23), null, null, null, "https://daihoc.fpt.edu.vn/wp-content/uploads/2023/04/cropped-cropped-2021-FPTU-Long.png", false),
                new Conference(_conference2ndSciId = _guidGenerator.Create(), "Second Science Conference", "2ndSci", "HoChiMinh", "VietNam", new DateTime(2023, 04, 23), new DateTime(2023, 08, 23), null, null, null, "https://daihoc.fpt.edu.vn/wp-content/uploads/2023/04/cropped-cropped-2021-FPTU-Long.png", false),
                new Conference(_conference3rdSciId = _guidGenerator.Create(), "Third Science Conference", "3rdSci", "HoChiMinh", "VietNam", new DateTime(2024, 04, 23), new DateTime(2024, 08, 23), null, null, null, "https://daihoc.fpt.edu.vn/wp-content/uploads/2023/04/cropped-cropped-2021-FPTU-Long.png", false),
                new Conference(_conferenceSingleTrackId = _guidGenerator.Create(), "Single Track Conference", "3rdSci", "HoChiMinh", "VietNam", new DateTime(2023, 04, 23), new DateTime(2023, 08, 23), null, null, null, "https://daihoc.fpt.edu.vn/wp-content/uploads/2023/04/cropped-cropped-2021-FPTU-Long.png", true),
            };

            await _conferenceRepository.InsertManyAsync(conferences);
        }

        private async Task CreateTrackConferenceAsync()
        {/* Add data for Track table */
            if (await _trackConferenceRepository.GetCountAsync() > 0)
            {
                return;
            }

            var tracks = new List<Track>
            {
                new Track(_track1stSci1Id = _guidGenerator.Create(), true, "Artificial Intelligence", _conference1stSciId, null, null, null, null, null, null),
                new Track(_track1stSci2Id = _guidGenerator.Create(), true, "Cloud Computing", _conference1stSciId, null, null, null, null, null, null),
                new Track(_track2ndSci1Id = _guidGenerator.Create(), true, "Programming with Alice", _conference2ndSciId, null, null, null, null, null, null),
                new Track(_track2ndSci2Id = _guidGenerator.Create(), true, "Artificial Intelligence", _conference2ndSciId, null, null, null, null, null, null),
                new Track(_track2ndSci3Id = _guidGenerator.Create(), true, "Programming Fundamentals", _conference2ndSciId, null, null, null, null, null, null),
                new Track(_track3rdSci1Id = _guidGenerator.Create(), true, "Cloud Computing", _conference3rdSciId, null, null, null, null, null, null),
                new Track(_track3rdSci2Id = _guidGenerator.Create(), true, "Artificial Intelligence", _conference3rdSciId, null, null, null, null, null, null),
                new Track(_track3rdSci3Id = _guidGenerator.Create(), true, "Programming with Alice", _conference3rdSciId, null, null, null, null, null, null),
                new Track(_trackSingleId = _guidGenerator.Create(), true, "Single Track", _conferenceSingleTrackId, null, null, null, null, null, null),
            };

            await _trackConferenceRepository.InsertManyAsync(tracks, autoSave: true);
        }

        private async Task CreateSubjectAreaAsync()
        {/* Add data for SubjectArea table */
            if (await _subjectAreaRepository.GetCountAsync() > 0)
            {
                return;
            }

            var subjectAreas = new List<SubjectArea>
            {
                new SubjectArea(_guidGenerator.Create(), "Mobile", _track2ndSci2Id),
                new SubjectArea(_guidGenerator.Create(), "Robot", _track2ndSci2Id),
                new SubjectArea(_guidGenerator.Create(), "Smart Home", _track2ndSci2Id),
                new SubjectArea(_guidGenerator.Create(), "Programming Fundamentals", _track2ndSci3Id),
                new SubjectArea(_guidGenerator.Create(), "Programming with Alice", _track2ndSci1Id),
                new SubjectArea(_guidGenerator.Create(), "Cloud Computing", _track3rdSci1Id),
                new SubjectArea(_guidGenerator.Create(), "Programming with Alice", _track3rdSci3Id),
                new SubjectArea(_guidGenerator.Create(), "Single Track Subject Area", _trackSingleId),
            };

            await _subjectAreaRepository.InsertManyAsync(subjectAreas, autoSave: true);
        }

        private async Task CreateEmailTemplateAsync()
        {/* Add data for EmailTemplate table */
            if (await _emailTemplateRepository.GetCountAsync() > 0)
            {
                return;
            }

            var emailTemplates = new List<EmailTemplate>
            {
                new EmailTemplate(_guidGenerator.Create(), "Template Revision", "Revision request notification email", "Please update your revision on time.", null, null),
                new EmailTemplate(_guidGenerator.Create(), "Template Accepted", "Your submission has been accepted", "Please check your submission's information and send your submission's last version on time.", _conference1stSciId, null),
                new EmailTemplate(_guidGenerator.Create(), "Template Accepted Track", "Your submission has been accepted", "Please check your submission's information and send your submission's last version on time.", _conference1stSciId, _track1stSci1Id),
                new EmailTemplate(_guidGenerator.Create(), "Template Accepted Track 2ndSci", "Your submission has been accepted", "Please check your submission's information and send your submission's last version on time.", _conference2ndSciId, _track1stSci2Id),
                new EmailTemplate(_guidGenerator.Create(), "Template Reject", "Your submission has been rejected" , "Thank you for your submission, but your submission does not match our requirements.", _conference1stSciId, null),
                new EmailTemplate(_guidGenerator.Create(), "Template Reject", "Your submission has been rejected" , "Thank you for your submission, but your submission does not match our requirements.", null, _track3rdSci2Id),
                new EmailTemplate(_guidGenerator.Create(), "Template Reject", "Your submission has been rejected" , "Thank you for your submission, but your submission does not match our requirements.", _conference3rdSciId, null),
            };

            await _emailTemplateRepository.InsertManyAsync(emailTemplates, autoSave: true);
        }

        private async Task CreateSubmissionAsync()
        {/* Add data for Submission table */
            if (await _submissionRepository.GetCountAsync() > 0)
            {
                return;
            }

            var submissions = new List<Submission>
            {
                new Submission(_guidGenerator.Create(), "First Science Paper", "This paper is a submission for the first Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track1stSci1Id, null, null, null, null, _paperStatusDeskReject, true, _paperStatusDeskReject, null, false),
                new Submission(_guidGenerator.Create(), "Second Science Paper", "This paper is a submission for the first Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track1stSci1Id, null, null, null, null, _paperStatusAccept, true, _paperStatusAccept, null, true),
                new Submission(_guidGenerator.Create(), "Third Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci1Id, null, null, null, null, _paperStatusAwaitingDecision, null, null, null, false),
                new Submission(_guidGenerator.Create(), "Fourth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci2Id, null, null, null, null, _paperStatusReject, true, _paperStatusReject, null, false),
                new Submission(_guidGenerator.Create(), "Fifth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci1Id, null, null, null, null, _paperStatusAwaitingDecision, null, null, null, false),
                new Submission(_guidGenerator.Create(), "Sixth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci1Id, null, null, null, null, _paperStatusAccept, false, null, null, false),
                new Submission(_guidGenerator.Create(), "Seventh Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci2Id, null, null, null, null, _paperStatusRevision, true, _paperStatusRevision, null, true),
                new Submission(_guidGenerator.Create(), "Eighth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci2Id, null, null, null, null, _paperStatusDeskReject, true, _paperStatusDeskReject, null, false),
                new Submission(_guidGenerator.Create(), "Ninth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci3Id, null, null, null, null, _paperStatusAccept, true, _paperStatusAccept, null, false),
                new Submission(_guidGenerator.Create(), "Tenth Science Paper", "This paper is a submission for the secound Science Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _track2ndSci3Id, null, null, null, null, _paperStatusWithdrawn, true, _paperStatusWithdrawn, null, false),
                new Submission(_guidGenerator.Create(), "Single Track Paper", "This paper is a submission for the Single Track Conference.", "https://cmt3.research.microsoft.com/api/ResFes2023/Files/356", _trackSingleId, null, null, null, null, _paperStatusAccept, true, _paperStatusAccept, null, false),
            };

            await _submissionRepository.InsertManyAsync(submissions, autoSave: true);
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

            if (await _paperStatusRepository.GetCountAsync() <= 1) await CreatePaperStatusesAsync();
            if (await _conferenceRoleRepository.GetCountAsync() <= 1) await CreateConferenceRolesAsync();
            if (await _placeholderGroupRepository.GetCountAsync() <= 1) await CreatePlaceholderGroupAsync();
            if (await _supportedPlaceholderRepository.GetCountAsync() <= 1) await CreateSupportedPlaceholderAsync();

            if (await _conferenceRepository.GetCountAsync() <= 1) await CreateConferenceAsync();
            if (await _trackConferenceRepository.GetCountAsync() <= 1) await CreateTrackConferenceAsync();

            if (await _subjectAreaRepository.GetCountAsync() <= 1) await CreateSubjectAreaAsync();
            if (await _emailTemplateRepository.GetCountAsync() <= 1) await CreateEmailTemplateAsync();

            if (await _submissionRepository.GetCountAsync() <= 1) await CreateSubmissionAsync();
            if (await _conflictCaseRepository.GetCountAsync() <= 1) await CreateConflictCaseAsync();
        }
    }
}
