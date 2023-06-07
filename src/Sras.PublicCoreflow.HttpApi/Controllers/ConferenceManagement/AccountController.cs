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

        public AccountController(IAccountAppService accountAppService)
        {
            _accountAppService = accountAppService;
        }

        [HttpGet("by-email/{email}")]
        public async Task<AccountWithBriefInfo?> GetAsync(string email)
        {
            return await _accountAppService.FindAsync(email);
        }
    }
}
