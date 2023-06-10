using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IEmailTemplateAppService
    {
        Task<object> GetEmailSendEachStatus(PaperStatusToEmail request);
        Task<IEnumerable<object>> GetEmailTemplateByConferenceIdOrTrackId(Guid conferenceId, Guid? trackId);
        Task<object> GetEmailTemplateById(Guid id);
    }
}
