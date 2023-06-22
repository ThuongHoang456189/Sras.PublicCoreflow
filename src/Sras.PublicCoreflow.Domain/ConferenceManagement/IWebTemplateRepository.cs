using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IWebTemplateRepository
    {
        Task<bool> CreateTemplate(Guid webTemplateId, string rootFilePath);
        Task<IEnumerable<object>> GetListWebTemplateName();
    }
}
