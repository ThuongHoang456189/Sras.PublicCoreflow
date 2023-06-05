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

        public async Task<List<AccountWithProfile>> GetAllAccountsWithProfileListAsync()
        {
            //var dbContext = await GetDbContextAsync();
            var userList = await _userRepository.ToListAsync();
            var finalResult = new List<AccountWithProfile>();

            var tasks = userList.Select(async user =>
            {
                var conferenceAccs = (await _conferenceAccountRepository.ToListAsync())
                    .Where(confAcc => confAcc.AccountId == user.Id)
                    .Select(confacc => confacc.Id.ToString("D"))
                    .ToList();
                var conferenceRoleIds = (await _incumbentRepository.ToListAsync())
                    .Where(incb => conferenceAccs.Contains(incb.ToString()))
                    .GroupBy(incc => incc.ConferenceRoleId)
                    .Select(i => i.First().ConferenceRoleId)
                    .Select(inc => inc.ToString())
                    .ToList();
                var conferenceRoleNames = (await _conferenceRoleRepository.ToListAsync())
                    .Where(confRole => conferenceRoleIds.Contains(confRole.Id.ToString()))
                    .Select(cf => cf.Name)
                    .ToList();
                var accountWithProfile = new AccountWithProfile()
                {
                    Id = user.Id,
                    Email = user?.Email,
                    FirstName = user?.Name,
                    MiddleName = user?.Name,
                    LastName = user?.Name,
                    Organization = user?.OrganizationUnits?.ToString(),
                    roles = conferenceRoleNames
                };
                return accountWithProfile;
            }).ToList();

            finalResult = (await Task.WhenAll(tasks)).ToList();

            return finalResult;
        }

        public async Task<List<ConferenceAccount>> TestGetAll()
        {
            return _incumbentRepository.ToListAsync();
            //TO DO
        }
    }
}
