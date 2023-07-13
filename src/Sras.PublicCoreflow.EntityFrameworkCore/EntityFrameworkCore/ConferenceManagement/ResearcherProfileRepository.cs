using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Polly;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using static Volo.Abp.Identity.IdentityPermissions;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ResearcherProfileRepository : EfCoreRepository<PublicCoreflowDbContext, ResearcherProfile, Guid>, IResearcherProfileRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        public ResearcherProfileRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<bool> hasResearchProfile(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.ResearcherProfiles.Include(r => r.Account).Any(r => r.Account.Id == userId);
        }

        public async Task<bool> isPrimaryEmailDuplicate(Guid userId,  string email)
        {
            var dbContext = await GetDbContextAsync();
            var thisRP = dbContext.ResearcherProfiles.Where(r => r.Account.Id == userId);
            if (dbContext.ResearcherProfiles.ToList().Except(thisRP).Any(r => r.PrimaryEmail == email)) return true;
            return false;
        }
        
        public async Task<bool> isAccountExist(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Users.Any(u => u.Id == userId);
        }

        // create generate profile
        public async Task<object> createGeneralProfile(GeneralProfileRequest request)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.Users.Any(u => u.Id == request.userId) == false) throw new Exception("userId not existing");
            ResearcherProfile researcherProfile = new ResearcherProfile(request.userId, 
                dbContext.Users.Find(request.userId), 
                request.publishName, 
                request.primaryEmail, 
                request.orcid, 
                request.introduction, 
                request.dateOfBirth, 
                request.gender, request.scientistTitle, request.adminPositionFunction, request.academicFunction, request.academic, request.currentDegree, request.current, request.homeAddress, request.phoneNumber, request.mobilePhone, request.fax);
            dbContext.ResearcherProfiles.Add(researcherProfile);
            dbContext.SaveChanges();
            return dbContext.ResearcherProfiles.Where(r => r.Id == request.userId).Select(r => new GeneralProfileRequest()
            {
                userId = r.Id,
                publishName = r.PublishName,
                primaryEmail = r.PrimaryEmail,
                orcid = r.ORCID,
                introduction = r.Introduction,
                dateOfBirth = r.DateOfBirth,
                gender = r.Gender,
                scientistTitle = r.CurrentResearchScientistTitle,
                adminPositionFunction = r.CurrentAdministrationPosition,
                academicFunction = r.CurrentAcademicFunction,
                academic = r.YearOfCurrentAcademicFunctionAchievement,
                currentDegree = r.CurrentDegree,
                current = r.YearOfCurrentCurrentDegreeAchievement,
                homeAddress = r.HomeAddress,
                phoneNumber = r.PhoneNumber,
                mobilePhone = r.MobilePhoneNumber,
                fax = r.Fax // 17 fields
            }).First();
        }

        // get general profile
        public async Task<object> GetGeneralProfile(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) return null;
            return dbContext.ResearcherProfiles.Where(r => r.Id == userId).Select(r => new GeneralProfileRequest()
                {
                    userId = r.Id,
                    publishName = r.PublishName,
                    primaryEmail = r.PrimaryEmail,
                    orcid = r.ORCID,
                    introduction = r.Introduction,
                    dateOfBirth = r.DateOfBirth,
                    gender = r.Gender,
                    scientistTitle = r.CurrentResearchScientistTitle,
                    adminPositionFunction = r.CurrentAdministrationPosition,
                    academicFunction = r.CurrentAcademicFunction,
                    academic = r.YearOfCurrentAcademicFunctionAchievement,
                    currentDegree = r.CurrentDegree,
                    current = r.YearOfCurrentCurrentDegreeAchievement,
                    homeAddress = r.HomeAddress,
                    phoneNumber = r.PhoneNumber,
                    mobilePhone = r.MobilePhoneNumber,
                    fax = r.Fax, // 17 fields,
                    otherIds = r.OtherIDs == null ? "[]" : r.OtherIDs,
                    websiteAndSocialLinks = r.WebsiteAndSocialLinks == null ? "[]" : r.WebsiteAndSocialLinks,
                    alsoKnownAs = r.AlsoKnownAs == null ? "[]" : r.AlsoKnownAs
                }).First();
        }

        // update  websiteAndSocialLinks
        public async Task<bool> UpdateWebsiteAndSocialLinks(Guid userId, string websiteAndSocialLinks)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            var profile = await dbContext.ResearcherProfiles.FindAsync(userId);
                profile.WebsiteAndSocialLinks = websiteAndSocialLinks;
                dbContext.SaveChanges();
                return true;
        }

        // update  alsoKnowas
        public async Task<bool> UpdateAlsoKnownAs(Guid userId, string alsoKnownAs)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            var profile = await dbContext.ResearcherProfiles.FindAsync(userId);
            profile.AlsoKnownAs = alsoKnownAs;
            dbContext.SaveChanges();
            return true;
        }

        // update othersId
        public async Task<bool> UpdateOthersId(Guid userId, string othersId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            var profile = await dbContext.ResearcherProfiles.FindAsync(userId);
            profile.OtherIDs = othersId;
            dbContext.SaveChanges();
            return true;
        }

        // get workplace
        public async Task<object> GetWorkPlace(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string workplaceStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Workplace;
            if (workplaceStr == null) return null;
            Organization workplace = JsonSerializer.Deserialize<Organization>(workplaceStr);
            return new
            {
                organizationId = workplace.organizationId,
                organizationName = workplace.organizationName,
                organizationDescription = workplace.organizationDescription,
                organizationWebsite = workplace.organizationWebsite,
                organizationPhoneNumber = workplace.organizationPhoneNumber,
                grid = workplace.grid
            };
        }

        // update workplace
        public async Task<bool> UpdateWorkplace(Guid userId, Organization organization)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Workplace = JsonSerializer.Serialize<Organization>(organization);
                return true;
            } catch (Exception ex)
            {
                return false;
            }
            
        }

        // get education
        public async Task<object> GetEducation(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Educations;
            if (educationStr == null) return new List<object>();
            List<Education> educations = JsonSerializer.Deserialize<List<Education>>(educationStr);
            return educations.Select(education => 
            new
            {
                education.educationId,
                education.academicDegreeId,
                academicDegreeName = JsonSerializer.Deserialize<List<ReferenceTypeDegree>>(File.ReadAllText(Directory.GetCurrentDirectory() + "\\Json\\ResearcherProfile\\academic-degree-level-reference-types.json")).Where(a => a.ReferenceTypeId == education.academicDegreeId).First().ReferenceTypeName,
                education.educationalOrganization,
                education.startYear,
                education.yearOfGraduation,
                education.degreeAbbreviation,
                education.degree
            });
        }

        // update education
        public async Task<bool> UpdateEducation(Guid userId, List<Education> education)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Educations = JsonSerializer.Serialize<List<Education>>(education);
                return true;
            } catch (Exception ex)
            {
                return false;
            }
        }

        // get Employments
        public async Task<object> GetEmployment(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Employments;
            if (educationStr == null) return new List<object>();
            List<Employment> education = JsonSerializer.Deserialize<List<Employment>>(educationStr);
            return education;
        }

        // update Employments
        public async Task<bool> UpdateEmployment(Guid userId, List<Employment> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Employments = JsonSerializer.Serialize<List<Employment>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get scholarship
        public async Task<object> GetScholarships(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Scholarships;
            if (educationStr == null) return new List<object>();
            List<ScholarshipAndAward> education = JsonSerializer.Deserialize<List<ScholarshipAndAward>>(educationStr);
            return education;
        }

        // update scholarship
        public async Task<bool> UpdateScholarships(Guid userId, List<ScholarshipAndAward> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Scholarships = JsonSerializer.Serialize<List<ScholarshipAndAward>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get award
        public async Task<object> GetAward(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Awards;
            if (educationStr == null) return new List<object>();
            List<ScholarshipAndAward> education = JsonSerializer.Deserialize<List<ScholarshipAndAward>>(educationStr);
            return education;
        }

        // update award
        public async Task<bool> UpdateAward(Guid userId, List<ScholarshipAndAward> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Awards = JsonSerializer.Serialize<List<ScholarshipAndAward>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get skill
        public async Task<object> GetSkill(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Skills;
            if (educationStr == null) return new List<object>();
            List<Skill> education = JsonSerializer.Deserialize<List<Skill>>(educationStr);
            return education;
        }

        // update skill
        public async Task<bool> UpdateSkill(Guid userId, List<Skill> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Skills = JsonSerializer.Serialize<List<Skill>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get research direction
        public async Task<object> GetResearchDirection(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.ResearchDirections;
            if (educationStr == null) return new List<object>();
            List<ResearchDirection> education = JsonSerializer.Deserialize<List<ResearchDirection>>(educationStr);
            return education;
        }

        // update research direction
        public async Task<bool> UpdateResearchDirection(Guid userId, List<ResearchDirection> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.ResearchDirections = JsonSerializer.Serialize<List<ResearchDirection>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // get publication
        public async Task<object> GetPublication(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Publications;
            if (educationStr == null) return new List<object>();
            List<Publication> educations = JsonSerializer.Deserialize<List<Publication>>(educationStr);
            return educations.Select(education =>
            new
            {
                education.publicationId,
                education.publicationName,
                education.publicationDate,
                education.publisher,
                education.doi,
                education.dOILink,
                education.publicationLinks,
                education.workTypeId,
                workTypeName = JsonSerializer.Deserialize<List<WorkTypeReferenceTypeDTO>>(File.ReadAllText(Directory.GetCurrentDirectory() + "\\Json\\ResearcherProfile\\work-type-reference-types.json")).Where(a => a.ReferenceTypeId == education.workTypeId).First().ReferenceTypeName,
                education.contributors,
                education.isLeadAuthor
            })
                ;
        }

        // update publication
        public async Task<bool> UpdatePublication(Guid userId, List<Publication> employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Publications = JsonSerializer.Serialize<List<Publication>>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
