﻿using Sras.PublicCoreflow.Domain.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailTemplateAppService : PublicCoreflowAppService, IEmailTemplateAppService
    {
        private readonly IEmailTemplateRepository _emailTemplateRespository;
        private readonly IGuidGenerator _guidGenerator;
        public EmailTemplateAppService(IEmailTemplateRepository emailTemplateRepository, IGuidGenerator guidGenerator) {
            _emailTemplateRespository = emailTemplateRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetEmailTemplateById(Guid id)
        {
            return await _emailTemplateRespository.GetEmailTemplateById(id);
        }

    }
}