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
        object CreateTemplate(RemoteStreamContent file, string name, string description, string fileName);
        ResponseDto CreateWebTemplateFiles(string filePath, RemoteStreamContent file);
        Task<IEnumerable<object>> GetListWebTemplateName(bool hasContent);
        Task<IEnumerable<byte[]>> downloadAllTemplates();
        Task<FileDTO> downloadOneTemplate(Guid templateId);
        Task<IEnumerable<object>> GetListWebTemplateFileInfo();
        Task<object> GetListTemplate();
        Task<object> UpdateTemplate(Guid webTemplateId, NavbarDTO navbarDTO);
    }
}
