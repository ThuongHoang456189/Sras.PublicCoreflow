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
        void CreateTemplate(Guid webTemplateId, string name, string description, string rootFilePath);
        Task<IEnumerable<string>> GetConferenceUsedByTemplateId(Guid id);
        Task<IEnumerable<object>> GetListWebTemplate();
        Task<IEnumerable<TemplateResponseDTO>> GetListWebTemplateName();
        TemplateResponseDTO GetTemplateById(Guid id);
        Task<object> UpdateTemplate(Guid webTemplateId, NavbarDTO navbarDTO);
    }
}
