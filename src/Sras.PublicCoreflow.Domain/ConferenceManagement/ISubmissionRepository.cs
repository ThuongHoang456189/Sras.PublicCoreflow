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

        Task<SubmissionReviewerAssignmentSuggestion> GetSubmissionReviewerAssignmentSuggestionAsync(Guid submissionId);

        Task<List<SubmissionAggregation>> GetListSubmissionAggregation(
            Guid conferenceId,
            Guid? trackId = null,
            string? sorting = SubmissionConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = SubmissionConsts.DefaultMaxResultCount);

        Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid conferenceId, Guid accountId);

        Task<List<SubmissionAggregationSP>> GetListSubmissionAggregationSP(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid? statusId,
            int skipCount,
            int maxResultCount
        );

        Task<SubmissionSummarySPO?> GetSubmissionSummaryAsync(Guid submissionId);

        Task<List<GetAuthorSubmissionAggregationSPO>?> GetAuthorSubmissionAggregationAsync(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid accountId,
            Guid? statusId,
            string? sorting,
            bool? sortedAsc,
            int skipCount,
            int maxResultCount);

        Task<List<GetReviewerSubmissionAggregationSPO>?> GetReviewerSubmissionAggregationAsync(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid accountId,
            bool? isReviewed,
            string? sorting,
            bool? sortedAsc,
            int skipCount,
            int maxResultCount);

        Task<List<GetSubmissionReviewerAssignmentSuggestionSPO>?> GetSubmissionReviewerAssignmentSuggestionAsync(
            string? inclusionText,
            Guid submissionId,
            bool? isAssigned);

        Task<List<GetSubmissionAggregationSPO>?> GetTopAverageScoreSubmissionAggregationAsync(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid? statusId,
            int skipCount,
            int maxResultCount);

        Task<List<GetSubmissionAggregationSPO>?> GetTopTimeSubmissionAggregationAsync(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid? statusId,
            int skipCount,
            int maxResultCount);

        Task<GetReviewerAssignmentSuggestionSubmissionPartSPO?> GetReviewerAssignmentSuggestionSubmissionPart(Guid submissionId);
    }
}
