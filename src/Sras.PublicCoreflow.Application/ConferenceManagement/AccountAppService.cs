using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Sras.PublicCoreflow.Extension;
using Volo.Abp.Data;
using System.Collections.Generic;
using System.Linq;
using Sras.PublicCoreflow.Dto;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AccountAppService : PublicCoreflowAppService, IAccountAppService
    {
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IRepository<Participant, Guid> _participantRepository;
        private readonly IIncumbentRepository _incumbentRepository;
        private readonly IRepository<ConferenceAccount, Guid> _conferenceAccountRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;
        private readonly IAccountRepository _accountRepository;
        private IGuidGenerator _guidGenerator;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IEmailRepository _emailRepository;

        public AccountAppService(IRepository<IdentityUser, Guid> userRepository,
            IRepository<Participant, Guid> participantRepository,
            IIncumbentRepository incumbentRepository,
            IRepository<ConferenceAccount, Guid> conferenceAccountRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository,
            IAccountRepository accountRepository,
            IGuidGenerator guidGenerator,
            IdentityUserManager identityUserManager,
            IEmailRepository emailRepository)
        {
            _userRepository = userRepository;
            _participantRepository = participantRepository;
            _incumbentRepository = incumbentRepository;
            _conferenceAccountRepository = conferenceAccountRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
            _accountRepository = accountRepository;
            _guidGenerator = guidGenerator;
            _identityUserManager = identityUserManager;
            _emailRepository = emailRepository;
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

        public IEnumerable<RegisterAccountRequest> GetAllAccount()
        {
            return _accountRepository.GetAllAccount(); 
        }

        public bool UpdateAccount(RegisterAccountRequest registerAccount)
        {
            _accountRepository.UpdateAccount(registerAccount);
            SendConfirmLinkThroughEmail(registerAccount.Email, registerAccount.Id, "Verify Account");
            return true;
        }

        public bool ConfirmEmail(Guid id)
        {
            if(_accountRepository.isConfirmAccount(id)) throw new Exception("Account Already Confirmed");
            return _accountRepository.ConfirmEmail(id);
        }

        public string SendConfirmLinkThroughEmail(string email, Guid nonConfirmedId, string subject)
        {
            return _emailRepository.SendEmailAsync(email, "http://localhost:3000/verify?account=" + nonConfirmedId, subject).Result;
        }
    }
}
