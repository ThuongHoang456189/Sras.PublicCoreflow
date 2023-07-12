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
    }
}
