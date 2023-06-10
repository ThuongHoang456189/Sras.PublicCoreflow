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
    public class ConflictRepository : EfCoreRepository<PublicCoreflowDbContext, Conflict, Guid>, IConflictRepository
    {
        public ConflictRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<List<ConflictWithDetails>> GetListReviewerConflictAsync(Guid incumbentId, Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            List<ConflictWithDetails> result = new List<ConflictWithDetails>();

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>() select i)
                                        .Where(x => x.Id == incumbentId && !x.IsDeleted);

            var conflictQueryable = (from c in dbContext.Set<Conflict>()
                                     join i in incumbentQueryable on c.IncumbentId equals i.Id
                                     join cc in dbContext.Set<ConflictCase>() on c.ConflictCaseId equals cc.Id
                                     select new ConflictWithDetails
                                     {
                                         SubmissionId = c.SubmissionId,
                                         IncumbentId = c.IncumbentId,
                                         ConflictCaseId = c.ConflictCaseId,
                                         ConflictCaseName = cc.Name,
                                         IsIndividualConflictCase = cc.IsIndividual,
                                         IsDefaultConflictCase = cc.IsDefault,
                                         TrackId = cc.TrackId,
                                         IsDefinedByReviewer = c.IsDefinedByReviewer
                                     })
                                     .Where(x => x.IncumbentId == incumbentId
                                     && x.SubmissionId == submissionId
                                     && x.IsDefinedByReviewer);

            var list = await conflictQueryable.ToListAsync();
            if(list.Any())
                result = list;

            return result;
        }

        public async Task<List<ReviewerConflictOperation>> GetReviewerConflictOperationTableAsync(Guid incumbentId, Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            List<ReviewerConflictOperation> result = new List<ReviewerConflictOperation>();

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>() select i)
                                        .Where(x => x.Id == incumbentId && !x.IsDeleted);

            var conflictQueryable = (from c in dbContext.Set<Conflict>()
                                     join i in incumbentQueryable on c.IncumbentId equals i.Id
                                     join cc in dbContext.Set<ConflictCase>() on c.ConflictCaseId equals cc.Id
                                     select new ReviewerConflictOperation
                                     {
                                         SubmissionId = c.SubmissionId,
                                         IncumbentId = c.IncumbentId,
                                         ConflictCaseId = c.ConflictCaseId,
                                         IsDefinedByReviewer = c.IsDefinedByReviewer,
                                         Operation = ReviewerConflictManipulationOperators.None
                                     })
                                     .Where(x => x.IncumbentId == incumbentId
                                     && x.SubmissionId == submissionId
                                     && x.IsDefinedByReviewer);

            var list = await conflictQueryable.ToListAsync();
            if (list.Any())
                result = list;

            return result;
        }
    }
}
