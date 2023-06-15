using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Threading.Tasks;
using System;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("SrasAccount")]
    [Route("api/sras/accounts")]
    public class AccountController : AbpController
    {
        private readonly IAccountAppService _accountAppService;
        private readonly IRegistrationAppService _registrationAppService;

        public AccountController(IAccountAppService accountAppService, IRegistrationAppService registrationAppService)
        {
            _accountAppService = accountAppService;
            _registrationAppService = registrationAppService;
        }

        [HttpGet("by-email/{email}")]
        public async Task<AccountWithBriefInfo?> GetAsync(string email)
        {
            return await _accountAppService.FindAsync(email);
        }

        [HttpPost("{id}/registrations")]
        public async Task<IActionResult> CreateRegistration(Guid id, Guid conferenceId, string mainPaperOption, List<RegistrationInput> registrations)
        {
            var resultDto = await _registrationAppService.CreateRegistration(id, conferenceId, mainPaperOption, registrations);
            if (resultDto.IsSuccessful)
            {
                return Ok(resultDto);
            }
            else
            {
                return BadRequest(resultDto);
            }
        }
    }
}
