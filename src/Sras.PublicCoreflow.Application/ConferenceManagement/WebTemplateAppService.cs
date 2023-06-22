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
    public class WebTemplateAppService : PublicCoreflowAppService, IWebTemplateAppService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebTemplateRepository _websiteRepository;

        public WebTemplateAppService(IGuidGenerator guidGenerator, IWebTemplateRepository websiteRepository)
        {
            _guidGenerator = guidGenerator;
            _websiteRepository = websiteRepository;
        }

        public async Task<IEnumerable<object>> GetListWebTemplateName()
        {
            return await _websiteRepository.GetListWebTemplateName();
        }

    }
}
