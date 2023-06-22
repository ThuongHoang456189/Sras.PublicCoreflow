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
        Task<ResponseDto> CreateTemplate(RemoteStreamContent file, string name, string description);
        Task<ResponseDto> CreateWebTemplateFiles(string filePath, RemoteStreamContent file);
        Task<IEnumerable<object>> GetListWebTemplateName(bool hasContent);
        Task<IEnumerable<byte[]>> downloadAllTemplates();
    }
}
