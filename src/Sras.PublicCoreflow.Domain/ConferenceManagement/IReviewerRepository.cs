using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerRepository : IRepository<Reviewer, Guid>
    {
        Task<Reviewer?> UpdateReviewerQuota(Guid accountId, Guid conferenceId, Guid trackId, int? quota);

        Task<Reviewer?> FindAsync(Guid accountId, Guid conferenceId, Guid trackId);
    }
}
