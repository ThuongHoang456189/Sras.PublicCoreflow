using AutoMapper.Internal.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;
using Sras.PublicCoreflow.Extension;
using static Sras.PublicCoreflow.ConferenceManagement.TrackAppService;
using Sras.PublicCoreflow.Dto;
using System.IO;
using Newtonsoft.Json;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ResearcherProfileAppService : PublicCoreflowAppService, IResearcherProfileAppService
    {
        private readonly IResearcherProfileRepository _repository;
        private readonly IEmailAppService _emailAppService;
        private readonly IResearcherProfileRepository _researcherProfileRepo;

        public ResearcherProfileAppService(IResearcherProfileRepository researcherProfileRepository, IEmailAppService emailAppService, IResearcherProfileRepository researcherProfileRepo)
        {
            _repository = researcherProfileRepository;
            _emailAppService = emailAppService;
            _researcherProfileRepo = researcherProfileRepo;
        }

        public async Task<bool> hasResearchProfile(Guid userId)
        {
            return await _repository.hasResearchProfile(userId);
        }

        public async Task<bool> isPrimaryEmailDuplicate(Guid userId, string email)
        {
            return await _repository.isPrimaryEmailDuplicate(userId, email);
        }

        public async Task<bool> isAccountExist(Guid userId)
        {
            return await _repository.isAccountExist(userId);
        }

        public async Task<bool> confirmPrimaryEmail(Guid userId, string email)
        {
            if (isAccountExist(userId).Result && email.Trim().Split("@").Last() == "fpt.edu.vn") {
                return true;
            }
            return false;
        }

        public async Task<bool> sendLinkConfirmAndCheckDuplicate(Guid userId, string email)
        {
                if (isPrimaryEmailDuplicate(userId, email).Result == false)
                {
                    _emailAppService.SendEmailAsync(email, "http://localhost:3000/create-profile?account=" + userId + "&email=" + email, "Confirm FPT Mail");
                    return true;
                } else
            {
                throw new Exception("Duplicate Primary Email");
            }

        }

        public async Task<object> createGeneralProfile(GeneralProfileRequest request)
        {
            return await _researcherProfileRepo.createGeneralProfile(request);
        }

        public async Task<object> GetGeneralProfile(Guid userId)
        {
            return new
            {
                result = await _researcherProfileRepo.GetGeneralProfile(userId)
            };
        }

        public async Task<bool> UpdateWebsiteAndSocialLinks(Guid userId, string websiteAndSocialLinks)
        {
            return await _researcherProfileRepo.UpdateWebsiteAndSocialLinks(userId, websiteAndSocialLinks);
        }

        public async Task<bool> UpdateAlsoKnownAs(Guid userId, string alsoKnownAs)
        {
            return await _researcherProfileRepo.UpdateAlsoKnownAs(userId, alsoKnownAs);
        }

        public async Task<bool> UpdateOthersId(Guid userId, string othersId)
        {
            return await _researcherProfileRepo.UpdateOthersId(userId, othersId);
        }

        public async Task<object> GetWorkPlace(Guid userId)
        {
            var result = await _researcherProfileRepo.GetWorkPlace(userId);
            return new
            {
                result
            };
        }

        public async Task<bool> UpdateWorkplace(Guid userId, Organization organization)
        {
            return await _researcherProfileRepo.UpdateWorkplace(userId, organization);
        }

        public async Task<object> GetEducation(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetEducation(userId));
        }

        public async Task<bool> UpdateEducation(Guid userId, List<Education> education)
        {
            return await _researcherProfileRepo.UpdateEducation(userId, education);
        }

        public async Task<object> GetEmployment(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetEmployment(userId));
        }

        public async Task<bool> UpdateEmployment(Guid userId, List<Employment> employment)
        {
            return await _researcherProfileRepo.UpdateEmployment(userId, employment);
        }

        public async Task<object> GetScholarships(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetScholarships(userId));
        }

        public async Task<bool> UpdateScholarships(Guid userId, List<ScholarshipAndAward> employment)
        {
            return await _researcherProfileRepo.UpdateScholarships(userId, employment);
        }

        public async Task<object> GetAward(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetAward(userId));
        }

        public async Task<bool> UpdateAward(Guid userId, List<ScholarshipAndAward> employment)
        {
            return await _researcherProfileRepo.UpdateAward(userId, employment);
        }

        public async Task<object> GetSkill(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetSkill(userId));
        }

        public async Task<bool> UpdateSkill(Guid userId, List<Skill> employment)
        {
            return await _researcherProfileRepo.UpdateSkill(userId, employment);
        }

        public async Task<object> GetResearchDirection(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetResearchDirection(userId));
        }

        public async Task<bool> UpdateResearchDirection(Guid userId, List<ResearchDirection> employment)
        {
            return await _researcherProfileRepo.UpdateResearchDirection(userId, employment);
        }

        public async Task<object> GetPublication(Guid userId)
        {
            return GetResultResponse(await _researcherProfileRepo.GetPublication(userId));
        }

        public async Task<bool> UpdatePublication(Guid userId, List<Publication> employment)
        {
            return await _researcherProfileRepo.UpdatePublication(userId, employment);
        }

        public async Task<object> GetAcademicDegreeLevelJson()
        {
            var root = Directory.GetCurrentDirectory();
            var text = File.ReadAllText(root + "\\Json\\ResearcherProfile\\academic-degree-level-reference-types.json");
            List<ReferenceTypeDegree> result = JsonConvert.DeserializeObject<List<ReferenceTypeDegree>>(text);
            return new
            {
                result
            };
        }

        public async Task<object> GetWorkTypeReferenceJson()
        {
            var root = Directory.GetCurrentDirectory();
            var text = File.ReadAllText(root + "\\Json\\ResearcherProfile\\work-type-reference-types.json");
            List<WorkTypeReferenceTypeDTO> result = JsonConvert.DeserializeObject<List<WorkTypeReferenceTypeDTO>>(text);
            return new
            {
                result
            };
        }

        public object GetResultResponse(object obj)
        {
            if (obj == null) return new
            {
                result = new List<object>()
            };

            return new
            {
                result = obj
            };
        }

    }
}
