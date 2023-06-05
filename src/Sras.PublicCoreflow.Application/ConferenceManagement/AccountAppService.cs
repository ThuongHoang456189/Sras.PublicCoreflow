﻿using System;
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
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;

        public AccountAppService(IRepository<IdentityUser, Guid> userRepository,
            IIncumbentRepository incumbentRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository)
        {
            _userRepository = userRepository;
            _incumbentRepository = incumbentRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
        }

        public async Task<AccountWithBriefInfo?> FindAsync(string email)
        {
            var user = await _userRepository.FindAsync(x => x.Email.Trim().ToLower().Equals(email.Trim().ToLower()));
            
            if(user == null) 
                return null;

            var result = ObjectMapper.Map<IdentityUser, AccountWithBriefInfo>(user);
            result.ParticipantId = user.GetProperty<Guid?>(nameof(result.ParticipantId));
            result.MiddleName = user.GetProperty<string?>(nameof(result.MiddleName));
            result.Organization = user.GetProperty<string?>(nameof(result.Organization));

            return result;
        }

        public async Task<List<ConferenceParticipationBriefInfo>> GetConferenceUserListAsync(ConferenceParticipationFilterDto filter)
        {
            return await _incumbentRepository.GetConferenceUserListAsync(filter.ConferenceId, filter.TrackId, filter.SkipCount, filter.MaxResultCount);
        }

    }
}
