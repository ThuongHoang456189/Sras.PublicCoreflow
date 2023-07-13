using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using System.Text.Json;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("ResearcherProfile")]
    [Route("api/sras/researcher-profile")]
    public class ResearcherProfileController : AbpController
    {
        private readonly IResearcherProfileAppService _appService;

        public ResearcherProfileController(IResearcherProfileAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{userId}/hasResearcherProfile")]
        public async Task<bool> hasResearchProfile(Guid userId)
        {
            return await _appService.hasResearchProfile(userId);
        }

        [HttpGet("confirm-primary-email/{userId}/{email}")]
        public async Task<bool> confirmPrimaryEmail(Guid userId, string email)
        {
            return await _appService.confirmPrimaryEmail(userId, email);
        }

        [HttpGet("send-link-confirm-and-check-duplicate/{userId}/{email}")]
        public async Task<ActionResult<bool>> sendLinkConfirmAndCheckDuplicate(Guid userId, string email)
        {
            try
            {
                var result = await _appService.sendLinkConfirmAndCheckDuplicate(userId, email);
                return Ok(result);
            } catch (Exception ex)
            {
                if (ex.Message == "Duplicate Primary Email") return Forbid(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> createGeneralProfile([FromBody] GeneralProfileRequest request)
        {
            try
            {
                return Ok(_appService.createGeneralProfile(request).Result);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<object>> GetGeneralProfile(Guid userId)
        {
            try
            {
                return Ok(await _appService.GetGeneralProfile(userId));
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-website-social-links/{userId}")]
        public async Task<ActionResult<bool>> UpdateWebsiteAndSocialLinks(Guid userId,[FromBody] StringBodyRequest websiteAndSocialLinks)
        {
            try
            {
                return Ok(_appService.UpdateWebsiteAndSocialLinks(userId, websiteAndSocialLinks.value).Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-also-known-as/{userId}")]
        public async Task<ActionResult<bool>> UpdateAlsoKnownAs(Guid userId,[FromBody] StringBodyRequest alsoKnownAs)
        {
            try
            {
                return Ok(_appService.UpdateAlsoKnownAs(userId, alsoKnownAs.value).Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("update-others-id/{userId}")]
        public async Task<ActionResult<bool>> UpdateOthersId(Guid userId, [FromBody] StringBodyRequest othersId)
        {
            try
            {
                return Ok(_appService.UpdateOthersId(userId, othersId.value).Result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-workplace/{userId}")]
        public async Task<object> GetWorkPlace(Guid userId)
        {
            return await _appService.GetWorkPlace(userId);
        }

        [HttpPatch("update-workplace/{userId}")]
        public async Task<bool> UpdateWorkplace(Guid userId,[FromBody] Organization organization)
        {
            return await _appService.UpdateWorkplace(userId, organization);
        }

        [HttpGet("get-education/{userId}")]
        public async Task<object> GetEducation(Guid userId)
        {
            return await _appService.GetEducation(userId);
        }

        [HttpPatch("update-education/{userId}")]
        public async Task<bool> UpdateEducation(Guid userId,[FromBody] List<Education> education)
        {
            return await _appService.UpdateEducation(userId, education);
        }

        [HttpGet("get-employment/{userId}")]
        public async Task<object> GetEmployment(Guid userId)
        {
            return await _appService.GetEmployment(userId);
        }

        [HttpPatch("update-employment/{userId}")]
        public async Task<bool> UpdateEmployment(Guid userId,[FromBody] List<Employment> employment)
        {
            return await _appService.UpdateEmployment(userId, employment);
        }

        [HttpGet("get-scholarships/{userId}")]
        public async Task<object> GetScholarships(Guid userId)
        {
            return await _appService.GetScholarships(userId);
        }

        [HttpPatch("update-scholarships/{userId}")]
        public async Task<bool> UpdateScholarships(Guid userId, [FromBody] List<ScholarshipAndAward> employment)
        {
            return await _appService.UpdateScholarships(userId, employment);
        }

        [HttpGet("get-award/{userId}")]
        public async Task<object> GetAward(Guid userId)
        {
            return await _appService.GetAward(userId);
        }

        [HttpPatch("update-award/{userId}")]
        public async Task<bool> UpdateAward(Guid userId,[FromBody] List<ScholarshipAndAward> employment)
        {
            return await _appService.UpdateAward(userId, employment);
        }

        [HttpGet("get-skill/{userId}")]
        public async Task<object> GetSkill(Guid userId)
        {
            return await _appService.GetSkill(userId);
        }

        [HttpPatch("update-skill/{userId}")]
        public async Task<bool> UpdateSkill(Guid userId,[FromBody] List<Skill> employment)
        {
            return await _appService.UpdateSkill(userId, employment);
        }

        [HttpGet("get-research-direction/{userId}")]
        public async Task<object> GetResearchDirection(Guid userId)
        {
            return await _appService.GetResearchDirection(userId);
        }

        [HttpPatch("update-research-direction/{userId}")]
        public async Task<bool> UpdateResearchDirection(Guid userId,[FromBody] List<ResearchDirection> employment)
        {
            return await _appService.UpdateResearchDirection(userId, employment);
        }

        [HttpGet("get-publication/{userId}")]
        public async Task<object> GetPublication(Guid userId)
        {
            return await _appService.GetPublication(userId);
        }

        [HttpPatch("update-publication/{userId}")]
        public async Task<bool> UpdatePublication(Guid userId,[FromBody] List<Publication> employment)
        {
            return await _appService.UpdatePublication(userId, employment);
        }

        [HttpGet("get-academic-degree-level-json")]
        public async Task<object> GetAcademicDegreeLevelJson()
        {
            var result =  JsonSerializer.Deserialize<List<ReferenceTypeDegree>>(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "\\Json\\ResearcherProfile\\academic-degree-level-reference-types.json"));
            return new
            {
                result
            };
        }

        [HttpGet("get-work-type-reference-json")]
        public async Task<object> GetWorkTypeReferenceJson()
        {
            var result = JsonSerializer.Deserialize<List<WorkTypeReferenceTypeDTO>>(System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "\\Json\\ResearcherProfile\\work-type-reference-types.json"));
            return new
            {
                result
            };
        }

    }

}

