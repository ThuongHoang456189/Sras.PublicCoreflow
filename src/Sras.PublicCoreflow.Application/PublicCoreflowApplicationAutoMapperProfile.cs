using AutoMapper;
using Sras.PublicCoreflow.ConferenceManagement;
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
    }
}
