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
        void createOriginTemplate(string name, string description);
        void CreateTemplate(Guid webTemplateId, string name, string description, string rootFilePath, NavbarDTO navbarDTO);
        Task<IEnumerable<string>> GetConferenceUsedByTemplateId(Guid id);
        Task<IEnumerable<object>> GetListWebTemplate();
        Task<IEnumerable<TemplateResponseDTO>> GetListWebTemplateName();
        TemplateResponseDTO GetTemplateById(Guid id);
        Task<Guid> getTemplateIdByWebId(string websiteId);
        Task<bool> RemoveTemplateByTemplateId(Guid templateId);
        Task<TemplateResponseDTO> UpdateTemplate(Guid webTemplateId, TemplateCreateRequestDTO dto);
    }
}
