using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebTemplateAppService
    {
        ResponseDto CreateWebTemplateFiles(string filePath, RemoteStreamContent file);
        Task<IEnumerable<object>> GetListWebTemplateName(bool hasContent);
        Task<IEnumerable<byte[]>> downloadAllTemplates();
        Task<FileDTO> downloadOneTemplate(Guid templateId);
        Task<IEnumerable<object>> GetListWebTemplateFileInfo();
        object GetListTemplate(string? websiteId);
        object CreateTemplate(string name, string description, NavbarDTO navbarDTO);
        Task<object> UpdateTemplate(Guid webTemplateId, TemplateCreateRequestDTO dto);
        Task<bool> DeleteWebTemplateById(Guid templateId);
    }
}
