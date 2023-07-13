using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IConferenceAppService : IApplicationService
    {
        Task<PagedResultDto<ConferenceWithBriefInfo>> GetListAsync(ConferenceListFilterDto filter);

        Task<ConferenceWithDetails> GetAsync(Guid id);

        // Can them chuc nang add Chair List
        Task<ConferenceWithDetails> CreateAsync(ConferenceWithDetailsInput input);

        Task<ConferenceWithDetails> UpdateAsync(Guid id, ConferenceWithDetailsInput input);

        Task<bool> DeleteAsync(Guid id);

        Task<PagedResultDto<ConferenceParticipationBriefInfo>> GetConferenceUserListAsync(Guid conferenceId, ConferenceParticipationFilterDto filter);
        Task<IEnumerable<object>> GetNumberOfSubmission(Guid conferenceId, Guid? trackId);
        Task<PriceTable?> GetPriceTable(Guid id);
        Task<object> GetConferenceDetail(Guid conferenceId);
        Task<object> GetConferenceAccountByAccIdConfId(Guid accId, Guid conferenceId);
        Task<IEnumerable<object>> GetConferencesWithNavbarStatus();
        Task<PagedResultDto<ConferenceUserDto>?> GetListConferenceUserAsync(Guid id, ConferenceUserInput input);
    }
}
