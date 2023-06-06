using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IIncumbentRepository : IRepository<Incumbent, Guid>
    {
        Task<bool> IsConferenceChair(Guid accountId, Guid conferenceId);

        Task<List<RoleTrackOperation>> GetRoleTrackOperationTableAsync(Guid accountId, Guid conferenceId);

        Task<ConferenceParticipationInfo?> GetConferenceParticipationInfoAsync(Guid accountId, Guid conferenceId, Guid? trackId);

        Task<List<ConferenceParticipationBriefInfo>> GetConferenceUserListAsync(Guid conferenceId, Guid? trackId, int skipCount = 0, int maxResultCount = int.MaxValue);

        Task<List<AuthorOperation>> GetAuthorOperationTableAsync(Guid conferenceId, Guid trackId, List<AuthorInput> authors);
    }
}
