using Scriban;
using Sras.PublicCoreflow.Domain.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp.Guids;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class EmailTemplateAppService : PublicCoreflowAppService, IEmailTemplateAppService
    {
        private readonly IEmailTemplateRepository _emailTemplateRespository;
        private readonly IPaperStatusRepository _paperStatusRespository;
        private readonly IGuidGenerator _guidGenerator;
        public EmailTemplateAppService(IEmailTemplateRepository emailTemplateRepository, IPaperStatusRepository paperStatusRepository, IGuidGenerator guidGenerator) {
            _emailTemplateRespository = emailTemplateRepository;
            _guidGenerator = guidGenerator;
            _paperStatusRespository = paperStatusRepository;
        }

        public async Task<object> GetEmailTemplateById(Guid id)
        {
            return await _emailTemplateRespository.GetEmailTemplateById(id);
        }

        public async Task<object> GetEmailTemplateByConferenceIdOrTrackId(Guid conferenceId, Guid? trackId)
        {
            var paperStatus = await _paperStatusRespository.GetPaperStatusesAllField(conferenceId);           
            if (trackId == null)
            {
                return new
                {
                    statuses = paperStatus.Select(ps => new
                    {
                        statusId = ps.Id,
                        name = ps.Name
                    }),
                    templates = await _emailTemplateRespository.GetEmailTemplateByConferenceId(conferenceId)
                };
            } else
            {
                return new
                {
                    statuses = paperStatus.Select(ps => new
                    {
                        statusId = ps.Id,
                        name = ps.Name
                    }),
                    templates = await _emailTemplateRespository.GetEmailTemplateByConferenceIdAndTrackId(conferenceId, trackId)
                };
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

        public async Task<object> UpdateEmailTemplate(UpdateEmailTemplateRequest request)
        {
            return await _emailTemplateRespository.UpdateEmailTempalte(request);
        }

    }
}
