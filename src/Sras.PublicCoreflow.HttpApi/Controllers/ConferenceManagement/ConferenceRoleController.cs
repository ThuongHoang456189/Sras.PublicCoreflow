using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ConferenceRole")]
    [Route("api/sras/conference-roles")]
    public class ConferenceRoleController : AbpController
    {
        private readonly IConferenceRoleAppService _conferenceRoleAppService;

        public ConferenceRoleController(IConferenceRoleAppService conferenceRoleAppService)
        {
            _conferenceRoleAppService = conferenceRoleAppService;
        }

        [HttpGet]
        public async Task<List<object>> GetConferenceRoles()
        {
            return await _conferenceRoleAppService.GetAllConferenceRole();
        }
    }
}
