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
            return new
            {
                result = dbContext.ResearcherProfiles.Where(r => r.Id == userId).Select(r => new GeneralProfileRequest()
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
                }).First()
            };
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
            Education education = JsonSerializer.Deserialize<Education>(educationStr);
            var educationalOrganization = new
            {
                education.educationalOrganization.organizationId,
                education.educationalOrganization.organizationName,
                education.educationalOrganization.organizationDescription,
                education.educationalOrganization.organizationWebsite,
                education.educationalOrganization.organizationPhoneNumber,
                education.educationalOrganization.grid
            };
            return new
            {
                education.educationId,
                education.academicDegreeId,
                academicDegreeName = JsonSerializer.Deserialize<List<ReferenceTypeDegree>>(File.ReadAllText("..\\Sras.PublicCoreflow.Domain.Shared\\Json\\ResearcherProfile")).Where(a => a.ReferenceTypeId == education.academicDegreeId).First().ReferenceTypeName,
                educationalOrganization,
                education.startYear,
                education.yearOfGraduation,
                education.degreeAbbreviation,
                education.degree
            };
        }

        // update education
        public async Task<bool> UpdateEducation(Guid userId, Education education)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Educations = JsonSerializer.Serialize<Education>(education);
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
            Employment education = JsonSerializer.Deserialize<Employment>(educationStr);
            return education;
        }

        // update Employments
        public async Task<bool> UpdateEmployment(Guid userId, Employment employment)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
                dbContext.ResearcherProfiles.FindAsync(userId).Result.Employments = JsonSerializer.Serialize<Employment>(employment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<object> GetScholarships(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.ResearcherProfiles.Any(r => r.Id == userId) == false) throw new Exception("userId not exist");
            string educationStr = dbContext.ResearcherProfiles.FindAsync(userId).Result.Scholarships;
            if (educationStr == null) return new List<object>();
            ScholarshipAndAward education = JsonSerializer.Deserialize<ScholarshipAndAward>(educationStr);
            return education;
        }

    }
}
