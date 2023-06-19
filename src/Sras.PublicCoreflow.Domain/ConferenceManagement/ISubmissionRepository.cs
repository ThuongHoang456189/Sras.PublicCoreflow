using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ISubmissionRepository : IRepository<Submission, Guid>
    {
        Task<object> GetNumOfSubmissionAndEmailWithAllAuthor(SubmissionWithEmailRequest request);
        Task<object> GetNumOfSubmissionAndEmailWithPrimaryContactAuthor(SubmissionWithEmailRequest request);
        Task<IEnumerable<object>> GetSubmissionAsync();
        Task<int> GetCountConflictedReviewer(Guid submissionId);
        Task<List<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid submissionId);

        Task<SubmissionReviewerAssignmentSuggestion> GeSubmissionReviewerAssignmentSuggestionAsync(Guid submissionId);

        Task<List<SubmissionAggregation>> GetListSubmissionAggregation(
            Guid conferenceId,
            Guid? trackId = null,
            string? sorting = SubmissionConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = SubmissionConsts.DefaultMaxResultCount);

        Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid conferenceId, Guid accountId);
    }
}
