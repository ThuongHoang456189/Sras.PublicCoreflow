using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

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

        //[HttpGet("test")]
        //public async Task<ConferenceWithDetails> CreateOrUpdateTestAsync(UserConferenceRoleInput input)
        //{
        //    return await _conferenceRoleAppService.CreateOrUpdateTestAsync(input);
        //}

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateAsync(UserConferenceRoleInput input)
        {
            return Ok( await _conferenceRoleAppService.CreateOrUpdateAsync(input) );
        }

        [HttpGet]
        public async Task<ConferenceParticipationInfo?> GetParticipationInfoAsync(ConferenceParticipationInput input)
        {
            return await _conferenceRoleAppService.GetConferenceParticipationInfoAsync(input);
        }
    }
}
