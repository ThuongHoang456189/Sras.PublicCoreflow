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
        Task<bool> DeleteNavbarAndHrefFile(Guid conferenceId, string idParent, string idChild);
        Task<IEnumerable<FileNameAndByteDTO>> DownloadAllFinalFile(Guid conferenceId);
        IEnumerable<FileNameAndByteDTO> ExportFinalFileOfWebsiteCreating(Guid webId, FileNameContentRequest[] fileNameContentRequests);
        Task<IEnumerable<object>> GetAllWebsite();
        Task<IEnumerable<object>> GetContentFinalOfWebsite(Guid conferenceId);
        Task<IEnumerable<object>> GetContentTempOfWebsite(Guid conferenceId);
        Task<object> getNavbarByConferenceId(Guid conferenceId);
        byte[] GetWebsiteFiles(string rootFilePath);
        Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO);
        Task<object> UpdatePageFile(Guid webId, string newPages);
        void UploadContentOfWebsite(Guid conferenceId, string fileName, string contentTemp, string contentFinal);
    }
}
