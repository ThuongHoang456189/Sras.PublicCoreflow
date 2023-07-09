using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ResearcherProfile : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Account))]
        public override Guid Id { get; protected set; }
        public IdentityUser Account { get; protected set; }
        public string PublishName { get; set; }
        public string PrimaryEmail { get; set; }
        public string? WebsiteAndSocialLinks { get; set; } // json -- by ReferenceWithReferenceTypeInclusion
        public string? ORCID { get; set; } // json -- by ReferenceWithReferenceTypeInclusion
        public string? OtherIDs { get; set; } // json -- by ReferenceWithReferenceTypeInclusion
        public string? AlsoKnownAs { get; set; } // json hoac list string, phan tach bang escape
        public string? Introduction { get; set; }
        public DateTime DateOfBirth { get; set; } //-just date only remove time
        public string Gender { get; set; }
        public string? CurrentResearchScientistTitle { get; set; }
        public string? CurrentAdministrationPosition { get; set; }
        public string? CurrentAcademicFunction { get; set; }
        public int? YearOfCurrentAcademicFunctionAchievement { get; set; }
        public string? CurrentDegree { get; set; }
        public int? YearOfCurrentCurrentDegreeAchievement { get; set; }
        public string? HomeAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobilePhoneNumber { get; set; }
        public string? Fax { get; set; }
        public string? Workplace { get; set; } // json Organization
        public string? Educations { get; set; } // json Education
        public string? Employments { get; set; } // json Employment
        public string? Scholarships { get; set; } // json --ScholarshipAndAward
        public string? Awards { get; set; } // json -- ScholarshipAndAward
        public string? Skills { get; set; } // json --Skill -- both for language and other skills
        public string? ResearchDirections { get; set; } // json ResearchDirection
        public string? Publications { get; set; } // json

        public ResearcherProfile(Guid id, string publishName, string primaryEmail, string? websiteAndSocialLinks, string? oRCID, string? otherIDs, string? alsoKnownAs, string? introduction, DateTime dateOfBirth, string gender, string? currentResearchScientistTitle, string? currentAdministrationPosition, string? currentAcademicFunction, int? yearOfCurrentAcademicFunctionAchievement, string? currentDegree, int? yearOfCurrentCurrentDegreeAchievement, string? homeAddress, string? phoneNumber, string? mobilePhoneNumber, string? fax, string? workplace, string? educations, string? employments, string? scholarships, string? awards, string? skills, string? researchDirections, string? publications) : base(id)
        {
            PublishName = publishName;
            PrimaryEmail = primaryEmail;
            WebsiteAndSocialLinks = websiteAndSocialLinks;
            ORCID = oRCID;
            OtherIDs = otherIDs;
            AlsoKnownAs = alsoKnownAs;
            Introduction = introduction;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            CurrentResearchScientistTitle = currentResearchScientistTitle;
            CurrentAdministrationPosition = currentAdministrationPosition;
            CurrentAcademicFunction = currentAcademicFunction;
            YearOfCurrentAcademicFunctionAchievement = yearOfCurrentAcademicFunctionAchievement;
            CurrentDegree = currentDegree;
            YearOfCurrentCurrentDegreeAchievement = yearOfCurrentCurrentDegreeAchievement;
            HomeAddress = homeAddress;
            PhoneNumber = phoneNumber;
            MobilePhoneNumber = mobilePhoneNumber;
            Fax = fax;
            Workplace = workplace;
            Educations = educations;
            Employments = employments;
            Scholarships = scholarships;
            Awards = awards;
            Skills = skills;
            ResearchDirections = researchDirections;
            Publications = publications;
        }
    }
}
