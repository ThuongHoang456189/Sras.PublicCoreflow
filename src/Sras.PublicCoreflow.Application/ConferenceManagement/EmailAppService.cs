using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailAppService : PublicCoreflowAppService, IEmailAppService
    {
        private readonly IEmailRepository _emailRepository;
        public EmailAppService(IEmailRepository emailRepository) {
            _emailRepository = emailRepository;
        }

        public async Task<object> SendEmailForEachStatus(PaperStatusToEmail request)
        {
            return await _emailRepository.SendEmailForEachStatus(request);
        }
    }
}
