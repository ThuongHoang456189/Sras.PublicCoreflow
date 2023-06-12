using Sras.PublicCoreflow.Domain.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
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

        public async Task<IEnumerable<object>> GetEmailTemplateByConferenceIdOrTrackId(Guid conferenceId, Guid? trackId)
        {
            if (trackId == null)
            {
                return await _emailTemplateRespository.GetEmailTemplateByConferenceId(conferenceId);
            } else
            {
                return await _emailTemplateRespository.GetEmailTemplateByConferenceIdAndTrackId(conferenceId, trackId);
            }
        }

        public async Task<object> GetEmailSendEachStatus(PaperStatusToEmail request)
        {
            return await _emailTemplateRespository.GetEmailSendEachStatus(request);
        }

        public async Task<object> CreateEmailTemplate(CreateEmailTemplateRequest request)
        {
            return await _emailTemplateRespository.CreateEmailTempate(request);
        }

    }
}
