using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IConflictRepository : IRepository<Conflict, Guid>
    {
        Task<List<ConflictWithDetails>> GetListReviewerConflictAsync(Guid incumbentId, Guid submissionId);

        Task<List<ConflictOperation>> GetReviewerConflictOperationTableAsync(Guid incumbentId, Guid submissionId);

        Task<List<ConflictOperation>> GetSubmissionConflictOperationTableAsync(Guid submissionId);
    }
}
