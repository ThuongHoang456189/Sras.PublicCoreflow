using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewAssignmentRepository : IRepository<ReviewAssignment, Guid>
    {
        Task<List<ReviewAssignment>> GetActiveReviewAssignment(Guid submissionCloneId);
    }
}
