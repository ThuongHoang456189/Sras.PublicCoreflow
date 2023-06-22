using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebTemplateAppService
    {
        ResponseDto CreateTemplate(RemoteStreamContent file);
        Task<IEnumerable<object>> GetListWebTemplateName();
    }
}
