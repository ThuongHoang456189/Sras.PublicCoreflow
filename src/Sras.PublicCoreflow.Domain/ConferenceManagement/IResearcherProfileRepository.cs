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
        Task<object> GetAward(Guid userId);
        Task<object> GetEducation(Guid userId);
        Task<object> GetEmployment(Guid userId);
        Task<object> GetGeneralProfile(Guid userId);
        Task<object> GetPublication(Guid userId);
        Task<object> GetResearchDirection(Guid userId);
        Task<object> GetScholarships(Guid userId);
        Task<object> GetSkill(Guid userId);
        Task<object> GetWorkPlace(Guid userId);
        Task<bool> hasResearchProfile(Guid userId);
        Task<bool> isAccountExist(Guid userId);
        Task<bool> isPrimaryEmailDuplicate(Guid userId, string email);
        Task<bool> UpdateAlsoKnownAs(Guid userId, string alsoKnownAs);
        Task<bool> UpdateAward(Guid userId, List<ScholarshipAndAward> employment);
        Task<bool> UpdateEducation(Guid userId, List<Education> education);
        Task<bool> UpdateEmployment(Guid userId, List<Employment> employment);
        Task<bool> UpdateOthersId(Guid userId, string othersId);
        Task<bool> UpdatePublication(Guid userId, List<Publication> employment);
        Task<bool> UpdateResearchDirection(Guid userId, List<ResearchDirection> employment);
        Task<bool> UpdateScholarships(Guid userId, List<ScholarshipAndAward> employment);
        Task<bool> UpdateSkill(Guid userId, List<Skill> employment);
        Task<bool> UpdateWebsiteAndSocialLinks(Guid userId, string websiteAndSocialLinks);
        Task<bool> UpdateWorkplace(Guid userId, Organization organization);
    }
}
