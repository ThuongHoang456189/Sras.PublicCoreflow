using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IEmailTemplateAppService
    {
        Task<object> CreateEmailTemplate(CreateEmailTemplateRequest request);
        Task<object> GetEmailSendEachStatus(PaperStatusToEmail request);
        Task<object> GetEmailTemplateByConferenceIdOrTrackId(Guid conferenceId, Guid? trackId);
        Task<object> GetEmailTemplateById(Guid id);
        Task<object> UpdateEmailTemplate(UpdateEmailTemplateRequest request);
    }
}
