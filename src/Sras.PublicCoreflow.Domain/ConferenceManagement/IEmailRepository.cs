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
        Task<object> SendEmailForEachStatus(PaperStatusToSendEmail request);
    }
}
