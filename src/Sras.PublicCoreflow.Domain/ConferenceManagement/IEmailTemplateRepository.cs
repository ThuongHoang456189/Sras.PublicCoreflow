using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.Domain.ConferenceManagement
{
    public interface IEmailTemplateRepository
    {
        Task<object> GetEmailTemplateById(Guid id);
    }
}
