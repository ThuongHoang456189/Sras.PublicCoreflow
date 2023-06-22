using AutoMapper.Internal.Mappers;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class WebsiteAppService : PublicCoreflowAppService, IWebsiteAppService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebsiteRepository _websiteRepository;

        public WebsiteAppService(IGuidGenerator guidGenerator, IWebsiteRepository websiteRepository)
        {
            _guidGenerator = guidGenerator;
            _websiteRepository = websiteRepository;
        }

        public async Task<object> getNavbarByConferenceId(Guid conferenceId)
        {
            return await _websiteRepository.getNavbarByConferenceId(conferenceId);
        }

        public async Task<object> CreateWebtemplate(String rootFilePath)
        {
            return await _websiteRepository.CreateWebtemplate(rootFilePath);
        }

        public async Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId)
        {
            return await _websiteRepository.CreateWebsite(webtemplateId, conferenceId);
        }

        public async Task<object> UpdateNavbarByConferenceId(Guid conferenceId, NavbarDTO navbarDTO)
        {
            return await _websiteRepository.UpdateNavbarByConferenceId(conferenceId, navbarDTO);
        }

    }
}
