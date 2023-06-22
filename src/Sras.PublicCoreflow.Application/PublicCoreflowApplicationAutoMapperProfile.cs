using AutoMapper;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow;

public class PublicCoreflowApplicationAutoMapperProfile : Profile
{
    public PublicCoreflowApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<Conference, ConferenceWithDetails>();
        CreateMap<ConferenceAccount, ConferenceAccountDto>();
        CreateMap<Incumbent, IncumbentDto>();
        CreateMap<IdentityUser, AccountWithBriefInfo>()
            .ForMember(a => a.FirstName,
            opt => opt.MapFrom(i => i.Name))
            .ForMember(a => a.LastName,
            opt => opt.MapFrom(i => i.Surname));
        CreateMap<Track, TrackBriefInfo>();
        CreateMap<SubjectArea, SubjectAreaBriefInfo>();
        CreateMap<SubmissionAggregationSP, SubmissionAggregationDto>()
            .ForMember(sd => sd.IsRevisionSubmitted,
            opt => opt.MapFrom(sp => sp.RevisionSubId != null))
            .ForMember(sd => sd.IsCameraReadySubmitted,
            opt => opt.MapFrom(sp => sp.CameraReadySubId != null))
            .ForMember(sd => sd.SubmissionConflicts,
            opt => opt.MapFrom(sp => sp.SubmissionConflicts == null ? 0 : sp.SubmissionConflicts))
            .ForMember(sd => sd.ReviewerConflicts,
            opt => opt.MapFrom(sp => sp.ReviewerConflicts == null ? 0 : sp.ReviewerConflicts))
            .ForMember(sd => sd.Assigned,
            opt => opt.MapFrom(sp => sp.Assigned == null ? 0 : sp.Assigned))
            .ForMember(sd => sd.Reviewed,
            opt => opt.MapFrom(sp => sp.Reviewed == null ? 0 : sp.Reviewed))
            .ForMember(sd => sd.AverageScore,
            opt => opt.MapFrom(sp => sp.AverageScore == null ? 0 : sp.AverageScore));

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();
    }
}
