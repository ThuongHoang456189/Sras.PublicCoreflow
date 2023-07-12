using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IResearcherProfileAppService
    {
        Task<bool> confirmPrimaryEmail(Guid userId, string email);
        Task<object> createGeneralProfile(GeneralProfileRequest request);
        Task<object> GetGeneralProfile(Guid userId);
        Task<bool> hasResearchProfile(Guid userId);
        Task<bool> isAccountExist(Guid userId);
        Task<bool> isPrimaryEmailDuplicate(Guid userId, string email);
        Task<bool> sendLinkConfirmAndCheckDuplicate(Guid userId, string email);
        Task<bool> UpdateAlsoKnownAs(Guid userId, string alsoKnownAs);
        Task<bool> UpdateOthersId(Guid userId, string othersId);
        Task<bool> UpdateWebsiteAndSocialLinks(Guid userId, string websiteAndSocialLinks);
    }
}
