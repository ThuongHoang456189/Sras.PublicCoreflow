using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebsiteRepository
    {
        void AddContentToWebsite(Guid webId, string fileName);
        Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId);
        Task<object> CreateWebtemplate(string rootFilePath);
        Task<object> getNavbarByConferenceId(Guid conferenceId);
        Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO);
    }
}
