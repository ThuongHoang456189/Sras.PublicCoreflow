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
        Task<bool> DeleteFileNameInPages(Guid webId, string fileName);
        Task<IEnumerable<string>> GetAllPageNameOfWebsite(Guid webId);
        Task<IEnumerable<object>> GetAllWebsite();
        Task<object> getNavbarByConferenceId(Guid conferenceId);
        Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO);
        Task<object> UpdatePageFile(Guid webId, string newPages);
    }
}
