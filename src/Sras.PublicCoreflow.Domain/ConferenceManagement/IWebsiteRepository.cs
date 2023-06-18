using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebsiteRepository
    {
        Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId);
        Task<object> CreateWebtemplate(string rootFilePath);
        Task<object> getNavbarByConferenceId(Guid conferenceId);
    }
}
