using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IResearcherProfileRepository
    {
        Task<bool> hasResearchProfile(Guid userId);
        Task<bool> isAccountExist(Guid userId);
        Task<bool> isPrimaryEmailDuplicate(Guid userId, string email);
    }
}
