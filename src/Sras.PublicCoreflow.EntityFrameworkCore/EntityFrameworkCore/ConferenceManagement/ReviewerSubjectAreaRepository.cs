using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ReviewerSubjectAreaRepository : EfCoreRepository<PublicCoreflowDbContext, ReviewerSubjectArea>, IReviewerSubjectAreaRepository
    {
        public ReviewerSubjectAreaRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ReviewerSubjectAreaOperation>> GetReviewerSubjectAreaOperationTableAsync(Guid reviewerId)
        {
            var dbContext = await GetDbContextAsync();

            var query = (from rsa in dbContext.Set<ReviewerSubjectArea>() 
                         select new ReviewerSubjectAreaOperation
                         {
                             ReviewerId = rsa.ReviewerId,
                             SubjectAreaId = rsa.SubjectAreaId,
                             IsPrimary = rsa.IsPrimary,
                             Operation = ReviewerSubjectAreaManipulationOperators.None
                         })
                        .Where(x => x.ReviewerId == reviewerId);

            return await query.ToListAsync();
        }
    }
}
