using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerRepository : IRepository<Reviewer, Guid>
    {
        Task<Reviewer?> UpdateReviewerQuota(Guid accountId, Guid conferenceId, Guid trackId, int? quota);
        Task<Reviewer?> FindAsync(Guid accountId, Guid conferenceId, Guid trackId);
        Task<Reviewer?> FindAsync(Guid id, Guid trackId);
        Task<int> GetCountReviewerAggregation(Guid accountId, Guid conferenceId);
        Task<List<SubmissionWithFacts>> GetListReviewerAggregation(
            Guid accountId, Guid conferenceId,
            string? sorting = ReviewerConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = ReviewerConsts.DefaultMaxResultCount);
    }
}
