using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IResearcherProfileRepository
    {
        Task<object> createGeneralProfile(GeneralProfileRequest request);
        Task<object> GetGeneralProfile(Guid userId);
        Task<bool> hasResearchProfile(Guid userId);
        Task<bool> isAccountExist(Guid userId);
        Task<bool> isPrimaryEmailDuplicate(Guid userId, string email);
        Task<bool> UpdateAlsoKnownAs(Guid userId, string alsoKnownAs);
        Task<bool> UpdateOthersId(Guid userId, string othersId);
        Task<bool> UpdateWebsiteAndSocialLinks(Guid userId, string websiteAndSocialLinks);
    }
}
