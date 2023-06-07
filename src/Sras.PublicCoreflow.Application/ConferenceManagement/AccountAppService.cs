using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Sras.PublicCoreflow.Extension;
using Volo.Abp.Data;
using System.Collections.Generic;
using System.Linq;
using Sras.PublicCoreflow.Dto;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AccountAppService : PublicCoreflowAppService, IAccountAppService
    {
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Participant, Guid> _participantRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;

        public AccountAppService(IRepository<IdentityUser, Guid> userRepository,
            IRepository<Participant, Guid> participantRepository,
            IIncumbentRepository incumbentRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository)
        {
            _userRepository = userRepository;
            _participantRepository = participantRepository;
            _incumbentRepository = incumbentRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
        }

        public async Task<AccountWithBriefInfo?> FindAsync(string email)
        {
            var user = await _userRepository.FindAsync(x => x.Email.Trim().ToLower().Equals(email.Trim().ToLower()));
            
            if(user == null) 
                return null;

            var participant = await _participantRepository.FindAsync(x => x.AccountId == user.Id);

            var result = ObjectMapper.Map<IdentityUser, AccountWithBriefInfo>(user);
            result.ParticipantId = participant == null ? null : participant.Id;
            result.MiddleName = user.GetProperty<string?>(nameof(result.MiddleName));
            result.Organization = user.GetProperty<string?>(nameof(result.Organization));
            result.Country = user.GetProperty<string?>(nameof(result.Country));

            return result;
        }

        public async Task<List<ConferenceParticipationBriefInfo>> GetConferenceUserListAsync(ConferenceParticipationFilterDto filter)
        {
            return await _incumbentRepository.GetConferenceUserListAsync(filter.ConferenceId, filter.TrackId, filter.SkipCount, filter.MaxResultCount);
        }

    }
}
