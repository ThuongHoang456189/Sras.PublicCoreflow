using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.Domain.ConferenceManagement
{
    public interface IEmailTemplateRepository
    {
        Task<object> CreateEmailTempate(CreateEmailTemplateRequest request);
        Task<object> GetEmailSendEachStatus(PaperStatusToEmail request);
        Task<IEnumerable<object>> GetEmailTemplateByConferenceId(Guid conferenceId);
        Task<IEnumerable<object>> GetEmailTemplateByConferenceIdAndTrackId(Guid conferenceId, Guid? trackId);
        Task<object> GetEmailTemplateById(Guid id);
    }
}
