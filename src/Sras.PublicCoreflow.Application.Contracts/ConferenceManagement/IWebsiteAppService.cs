using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebsiteAppService
    {
        Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId);
        Task<IEnumerable<object>> GetAllWebsite();
        Task<IEnumerable<object>> GetContentTempOfWebsite(Guid conferenceId);
        Task<object> getNavbarByConferenceId(Guid conferenceId);
        byte[] GetTemplateFiles(string rootFilePath);
        Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO);
        void UploadContentOfWebsite(Guid conferenceId, string fileName, string contentTemp, string contentFinal);
    }
}
