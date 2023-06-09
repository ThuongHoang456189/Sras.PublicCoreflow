using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IReviewerSubjectAreaRepository : IRepository<ReviewerSubjectArea>
    {
        Task<List<ReviewerSubjectAreaOperation>> GetReviewerSubjectAreaOperationTableAsync(Guid reviewerId);
    }
}
