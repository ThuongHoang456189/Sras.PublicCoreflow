using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebsiteAppService
    {
        Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId);
        Task<object> CreateWebtemplate(string rootFilePath);
        Task<object> getNavbarByConferenceId(Guid conferenceId);
    }
}
