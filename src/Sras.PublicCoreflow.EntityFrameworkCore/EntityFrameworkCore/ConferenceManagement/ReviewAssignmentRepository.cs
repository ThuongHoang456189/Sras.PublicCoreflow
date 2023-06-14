using Sras.PublicCoreflow.ConferenceManagement;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ReviewAssignmentRepository : EfCoreRepository<PublicCoreflowDbContext, ReviewAssignment, Guid>, IReviewAssignmentRepository
    {
        public ReviewAssignmentRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ReviewAssignment>> GetActiveReviewAssignment(Guid submissionCloneId)
        {
            var dbContext = await GetDbContextAsync();

            var query = (from r in dbContext.Set<Reviewer>()
                         join ra in dbContext.Set<ReviewAssignment>() on r.Id equals ra.ReviewerId
                         join i in dbContext.Set<Incumbent>() on r.Id equals i.Id
                         select ra)
                                     .Where(x => x.IsActive && x.SubmissionCloneId == submissionCloneId);

            return await query.ToListAsync();
        }
    }
}
