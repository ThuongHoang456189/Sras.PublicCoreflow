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

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ResearcherProfileAppService : PublicCoreflowAppService, IResearcherProfileAppService
    {
        private readonly IResearcherProfileRepository _repository;
        private readonly IEmailAppService _emailAppService;

        public ResearcherProfileAppService(IResearcherProfileRepository researcherProfileRepository, IEmailAppService emailAppService)
        {
            _repository = researcherProfileRepository;
            _emailAppService = emailAppService;
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
                    _emailAppService.SendEmailAsync(email, "http://localhost:3000/sciencetic-profile?account=" + userId + "&email=" + email, "Confirm FPT Mail");
                    return true;
                } else
            {
                throw new Exception("Duplicate Primary Email");
            }

        }

    }
}
