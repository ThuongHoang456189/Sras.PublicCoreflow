using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IEmailAppService
    {
        Task<object> SendEmailForEachStatus(PaperStatusToEmail request);
    }
}
