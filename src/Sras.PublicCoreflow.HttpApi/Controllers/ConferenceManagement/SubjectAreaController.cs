using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Conference")]
    [Route("api/sras/subject-areas")]
    public class SubjectAreaController : AbpController
    {
        private readonly ISubjectAreaAppService _subjectAreaAppService;

        [HttpGet]
        public async Task<List<SubjectAreaBriefInfo>> GetAll(Guid trackId)
        {
            return await _subjectAreaAppService.GetListAsync(trackId);
        }

        [HttpPost]
        public async Task Create(SubjectAreaInput input)
        {
            await _subjectAreaAppService.CreateAsync(input);
        }

        [HttpPost("{id}")]
        public async Task Update(Guid id, SubjectAreaInput input)
        {
            await _subjectAreaAppService.UpdateAsync(id, input); 
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _subjectAreaAppService.DeleteAsync(id);
        }
    }
}
