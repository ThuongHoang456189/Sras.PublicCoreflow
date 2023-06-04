using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using Volo.Abp;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceAppService : PublicCoreflowAppService, IConferenceAppService
    {
        private readonly IConferenceRepository _conferenceRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;
        private readonly IRepository<Incumbent, Guid> _incumbentRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IdentityUserAppService _userAppService;

        private const string Chair = "Chair";
        private const string DefaultTrackName = "__";
        private const string Admin = "admin";

        public ConferenceAppService(
            IConferenceRepository conferenceRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<Incumbent, Guid> incumbentRepository,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator,
            IdentityUserAppService userAppService)
        {
            _conferenceRepository = conferenceRepository;
            _userRepository = userRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _incumbentRepository = incumbentRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
            _userAppService = userAppService;
        }

        private async Task<bool> IsConferenceExist(ConferenceWithDetailsInput input)
        {
            return await _conferenceRepository.IsExistAsync(input.FullName, input.ShortName, input.City, input.Country, input.StartDate, input.EndDate);
        }

        private async Task CheckValidAccountIdAsync(Guid id)
        {
            var user = await _userAppService.GetAsync(id);
            if (user == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.InvalidAccountOnChairList);
            }
        }

        //[Authorize]
        public async Task<ConferenceWithDetails> CreateAsync(ConferenceWithDetailsInput input)
        {
            // Insert Authorize by role Admin

            var check = await IsConferenceExist(input);
            if (check)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceAlreadyExist);
            }
            else if (input.Chairs.IsNullOrEmpty())
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.NoAssignmentOfChairsToConference);
            }

            input.Chairs.ForEach(async x =>
            {
                var userQueryable = await _userRepository.GetQueryableAsync();
                var user = userQueryable.Where(u => u.Id == x.AccountId);
                if (user == null)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.InvalidAccountOnChairList);
                }
            });

            var conferenceId = _guidGenerator.Create();

            // Bo sung storage for logo img
            var conference = new Conference(conferenceId,
                    input.FullName, input.ShortName, input.City,
                    input.Country, input.StartDate, input.EndDate,
                    null, null, null, "logotemp1", input.IsSingleTrack);

            //var chairRole = await _conferenceRoleRepository.FindAsync(x => x.Name.EqualsIgnoreCase("chair"));

            var chairRole = await _conferenceRoleRepository.FindAsync(x => x.Name.Equals(Chair));

            bool isPrimaryContactSet = false;

            for (int i = 0; i < input.Chairs.Count; i++)
            {

                for (int j = i + 1; j < input.Chairs.Count; j++)
                {
                    if (input.Chairs[i].AccountId == input.Chairs[j].AccountId)
                    {
                        input.Chairs.Remove(input.Chairs[j]);
                        j--;
                    }
                }

                var conferenceAccount = new ConferenceAccount(_guidGenerator.Create(), conferenceId, input.Chairs[i].AccountId, false);
                conferenceAccount.AddIncumbent(_guidGenerator.Create(), chairRole.Id, null, isPrimaryContactSet ? false : input.Chairs[i].IsPrimaryContact);

                conference.AddConferenceAccount(conferenceAccount);

                isPrimaryContactSet = isPrimaryContactSet || input.Chairs[i].IsPrimaryContact;
            }

            // Clean track input
            if(input.Tracks != null)
                input.Tracks = input.Tracks.Distinct().ToList();

            if (input.IsSingleTrack)
            {
                var newTrack = new Track(_guidGenerator.Create(), true, DefaultTrackName, conferenceId, null, null, null, null, null, null);

                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.PaperSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.EditSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.SupplementaryMaterial, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.ReviewSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.AuthorNotification, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.RevisionSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.CameraReadySubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));

                conference.AddTrack(newTrack);
            }
            else
            {
                if(input.Tracks != null)
                {
                    input.Tracks.ForEach(x =>
                    {
                        var newTrack = new Track(_guidGenerator.Create(), false, x, conferenceId, null, null, null, null, null, null);

                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.PaperSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.EditSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.SupplementaryMaterial, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.ReviewSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.AuthorNotification, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.RevisionSubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));
                        newTrack.ActivityDeadlines.Add(new ActivityDeadline(_guidGenerator.Create(), ActivityDeadlineConsts.CameraReadySubmission, ActivityDeadlineConsts.Disabled, null, newTrack.Id));

                        conference.AddTrack(newTrack);
                    });
                }
            }

            return ObjectMapper.Map<Conference, ConferenceWithDetails>(await _conferenceRepository.InsertAsync(conference, true));
        }

        //[Authorize]
        public async Task<bool> DeleteAsync(Guid id)
        {
            var conference = await _conferenceRepository.FindAsync(x => x.Id == id);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            // Comment to simulate unauthorized access
            //else if (!IsAuthorizedToUpdateOrDelete(conference))
            //{
            //    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToDeleteConference);
            //}

            conference.ConferenceAccounts.Clear();
            conference.Tracks.Clear();
            await _conferenceRepository.DeleteAsync(conference, true);

            if (await _conferenceRepository.FindAsync(x => x.Id == id) == null)
                return true;
            return false;
        }

        public async Task<ConferenceWithDetails> GetAsync(Guid id)
        {
            var conference = await _conferenceRepository.FindAsync(x => x.Id == id);

            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            return ObjectMapper.Map<Conference, ConferenceWithDetails>(conference);
        }

        public async Task<PagedResultDto<ConferenceWithBriefInfo>> GetListAsync(ConferenceListFilterDto filter)
        {
            if (filter.MaxResultCount > ConferenceConsts.MaxResultCount)
                filter.MaxResultCount = ConferenceConsts.MaxResultCount;

            var totalCount = await _conferenceRepository.GetCountAsync(
                filter.InclusionText,
                filter.FullName,
                filter.ShortName,
                filter.City,
                filter.Country,
                filter.StartDate,
                filter.EndDate,
                filter.AccountId);

            var conferences = await _conferenceRepository.GetListAsync(
                string.IsNullOrWhiteSpace(filter.Sorting) ? null : filter.Sorting,
                filter.SkipCount,
                filter.MaxResultCount,
                filter.InclusionText,
                filter.FullName,
                filter.ShortName,
                filter.City,
                filter.Country,
                filter.StartDate,
                filter.EndDate,
                filter.AccountId);

            return new PagedResultDto<ConferenceWithBriefInfo>(totalCount, conferences);
        }

        private bool IsAuthorizedToUpdateOrDelete(Conference conference)
        {
            return _currentUser.Id == conference.CreatorId || _currentUser.IsInRole(Admin);
        }

        //[Authorize]
        public async Task<ConferenceWithDetails> UpdateAsync(Guid id, ConferenceWithDetailsInput input)
        {
            var conference = await _conferenceRepository.FindAsync(x => x.Id == id);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            // Disable because not authorize simulation
            //else if (!IsAuthorizedToUpdateOrDelete(conference))
            //{
            //    throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToUpdateConference);
            //}

            conference.SetFullName(input.FullName);
            conference.SetShortName(input.ShortName);
            conference.SetCity(input.City);
            conference.SetCountry(input.Country);
            conference.StartDate = input.StartDate;
            conference.EndDate = input.EndDate;
            conference.WebsiteLink = input.WebsiteLink;
            conference.Logo = input.Logo;
            conference.IsSingleTrack = true;

            // 1. Clean Chair List Input
            //input.Chairs.ForEach(async x => await CheckValidAccountIdAsync(x.AccountId));

            var chairRole = await _conferenceRoleRepository.FindAsync(x => x.Name.Equals(Chair));

            bool isPrimaryContactSet = false;

            for (int i = 0; i < input.Chairs.Count; i++)
            {
                for (int j = i + 1; j < input.Chairs.Count; j++)
                {
                    if (input.Chairs[i].AccountId == input.Chairs[j].AccountId)
                        input.Chairs.Remove(input.Chairs[j]);
                }

                if (isPrimaryContactSet && input.Chairs[i].IsPrimaryContact)
                    input.Chairs[i].IsPrimaryContact = false;

                isPrimaryContactSet = isPrimaryContactSet || input.Chairs[i].IsPrimaryContact;
            }

            //Check valid account
            input.Chairs.ForEach(async x =>
            {
                var userQueryable = await _userRepository.GetQueryableAsync();
                var user = userQueryable.Where(u => u.Id == x.AccountId);
                if (user == null)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.InvalidAccountOnChairList);
                }
            });

            // 2. Allocate operations
            var incumbentOperationTable = await _conferenceRepository.GetIncumbentOperationTableAsync(id);
            List<IncumbentOperation> chairTable = incumbentOperationTable.FindAll(x => x.ConferenceRoleId == chairRole.Id);
            List<IncumbentOperation> nonChairTable = incumbentOperationTable.FindAll(x => x.ConferenceRoleId != chairRole.Id);

            input.Chairs.ForEach(x =>
            {
                if (!incumbentOperationTable.Any(y => y.AccountId == x.AccountId))
                {
                    // Add2
                    chairTable.Add(new IncumbentOperation(_guidGenerator.Create(), x.AccountId, _guidGenerator.Create(), chairRole.Id, IncumbentManipulationOperators.Add2, x.IsPrimaryContact));
                }
                else
                {
                    // Up
                    var chairRow = chairTable.Find(y => y.AccountId == x.AccountId);
                    if (chairRow != null)
                    {
                        chairRow.Operation = IncumbentManipulationOperators.Up2;
                        chairRow.IsPrimaryContact = x.IsPrimaryContact;
                    }
                    else
                        chairTable.Add(new IncumbentOperation(_guidGenerator.Create(), x.AccountId, _guidGenerator.Create(), chairRole.Id, IncumbentManipulationOperators.UpAdd, x.IsPrimaryContact));
                }
            });

            chairTable.ForEach(x =>
            {
                if (!input.Chairs.Any(y => y.AccountId == x.AccountId))
                {
                    // Del
                    if (nonChairTable.Any(y => y.AccountId == x.AccountId))
                        x.Operation = IncumbentManipulationOperators.UpDel;
                    else
                        x.Operation = IncumbentManipulationOperators.Del2;
                }
            });

            // Refactor code merge step 2 and 3 after debug
            // 3. Update chair list
            chairTable.ForEach(x =>
            {
                if (x.Operation == IncumbentManipulationOperators.Add2)
                {
                    var chairIncumbent = new Incumbent(x.IncumbentId, x.ConferenceAccountId, chairRole.Id, null, x.IsPrimaryContact);
                }
                else if (x.Operation == IncumbentManipulationOperators.Up2)
                {
                    var updatedConferenceAccount = conference.ConferenceAccounts.SingleOrDefault(y => y.Id == x.ConferenceAccountId);
                    if (updatedConferenceAccount != null)
                    {
                        updatedConferenceAccount.UpdateIncumbent(x.IncumbentId, x.IsPrimaryContact);
                        conference.UpdateConferenceAccount(updatedConferenceAccount);
                    }
                }
                else if (x.Operation == IncumbentManipulationOperators.UpAdd)
                {
                    var updatedConferenceAccount = conference.ConferenceAccounts.SingleOrDefault(y => y.Id == x.ConferenceAccountId);
                    if (updatedConferenceAccount != null)
                    {
                        updatedConferenceAccount.AddIncumbent(x.IncumbentId, chairRole.Id, null, x.IsPrimaryContact);
                        conference.UpdateConferenceAccount(updatedConferenceAccount);
                    }
                }
                else if (x.Operation == IncumbentManipulationOperators.UpDel)
                {
                    var updatedConferenceAccount = conference.ConferenceAccounts.SingleOrDefault(y => y.Id == x.ConferenceAccountId);
                    if (updatedConferenceAccount != null)
                    {
                        updatedConferenceAccount.DeleteIncumbent(x.IncumbentId);
                        conference.UpdateConferenceAccount(updatedConferenceAccount);
                    }
                }
                else if (x.Operation == IncumbentManipulationOperators.Del2)
                {
                    conference.DeleteConferenceAccount(x.ConferenceAccountId);
                }
            });

            return ObjectMapper.Map<Conference, ConferenceWithDetails>(await _conferenceRepository.UpdateAsync(conference, true));
        }
    }
}
