using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IEmailRepository
    {
        Task<string> SendEmailAsync(string toEmails, string body, string subject);
        Task<object> SendEmailForEachStatus(PaperStatusToSendEmail request);
    }
}
