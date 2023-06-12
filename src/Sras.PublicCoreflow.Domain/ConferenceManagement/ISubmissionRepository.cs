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
    }
}
