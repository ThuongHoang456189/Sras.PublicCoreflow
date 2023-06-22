using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebTemplateAppService
    {
        Task<IEnumerable<object>> GetListWebTemplateName();
        Task<ResponseDto> CreateTemplate(RemoteStreamContent file);
        Task<ResponseDto> CreateWebTemplateFiles(string filePath, RemoteStreamContent file);
    }
}
