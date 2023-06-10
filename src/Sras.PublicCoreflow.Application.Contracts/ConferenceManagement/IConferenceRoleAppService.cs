using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IConferenceRoleAppService : IApplicationService
    {
        //Task<ConferenceWithDetails> CreateOrUpdateTestAsync(UserConferenceRoleInput input);

        Task<ResponseDto> CreateOrUpdateAsync(UserConferenceRoleInput input);

        Task<ConferenceParticipationInfo?> GetConferenceParticipationInfoAsync(ConferenceParticipationInput input);

        Task<List<object>> GetAllConferenceRole();
    }
}
