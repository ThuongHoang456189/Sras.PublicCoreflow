using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class SubmissionRepository : EfCoreRepository<PublicCoreflowDbContext, Submission, Guid>, ISubmissionRepository
    {
        private readonly IGuidGenerator _guidGenerator;

        public SubmissionRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetNumOfSubmissionAndEmailWithAllAuthor(SubmissionWithEmailRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Tracks.Any(t => t.Id == request.trackId))
                    throw new Exception("TrackId not existing");
                if (!request.paperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.paperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.trackId && s.StatusId == ps);
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        name = statusName,
                        numberOfSubmissions = submissionOfOneStatus,
                        numberOfEmail = numOfAllAuthor
                    };
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<object> GetNumOfSubmissionAndEmailWithPrimaryContactAuthor(SubmissionWithEmailRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Tracks.Any(t => t.Id == request.trackId))
                    throw new Exception("TrackId not existing");
                if (!request.paperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.paperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.trackId && s.StatusId == ps);
                    var numOfPrimaryAuthor = submissions.SelectMany(ss => ss.Authors).Where(au => au.IsPrimaryContact).Count();
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        name = statusName,
                        numberOfSubmissions = submissionOfOneStatus,
                        numberOfEmail = numOfPrimaryAuthor,
                    };
                });

                return result;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetSubmissionAsync()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Submissions.Select(s => new
            {
                Id = s.Id,
                Title = s.Title,
                TrackId = s.TrackId,
                StatusId = s.StatusId,
                isNotified = s.IsNotified,
                NotifiedStatusId = s.NotifiedStatusId,
                NotifiedStatusName = s.NotifiedStatus.Name
            }).ToList();
        }

        public async Task<int> GetCountConflictedReviewer(Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            var submission = await FindAsync(submissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>()
                                      join r in dbContext.Set<Reviewer>() on i.Id equals r.Id
                                      select i)
                                        .Where(x => x.TrackId == submission.TrackId);
            
            return await incumbentQueryable.CountAsync();
        }

        public async Task<List<ReviewerWithConflictDetails>> GetListReviewerWithConflictDetails(Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            var submission = await FindAsync(submissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            List<ReviewerWithConflictDetails> result = new List<ReviewerWithConflictDetails>();

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>()
                                      join r in dbContext.Set<Reviewer>() on i.Id equals r.Id
                                      select i)
                                        .Where(x => x.TrackId == submission.TrackId);

            var accountQueryable = (from ca in dbContext.Set<ConferenceAccount>()
                                    join i in incumbentQueryable on ca.Id equals i.ConferenceAccountId
                                    join a in dbContext.Set<IdentityUser>() on ca.AccountId equals a.Id
                                    select new
                                    {
                                        AccountId = a.Id,
                                        ReviewerId = i.Id
                                    });

            var accountList = await accountQueryable.ToListAsync();

            accountList.ForEach(x =>
            {
                var user = dbContext.Set<IdentityUser>().First(y => y.Id == x.AccountId);

                if(user != null)
                {
                    var conflictQueryable = (from c in dbContext.Set<Conflict>()
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
                                         .Where(y => y.IncumbentId == x.ReviewerId
                                         && y.SubmissionId == submissionId
                                         && !y.IsDefinedByReviewer);

                    ReviewerWithConflictDetails reviewer = new ReviewerWithConflictDetails
                    {
                        ReviewerId = x.ReviewerId,
                        FirstName = user.Name,
                        MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName),
                        LastName = user.Surname,
                        Email = user.Email,
                        Organization = user.GetProperty<string?>(AccountConsts.OrganizationPropertyName),
                        Conflicts = conflictQueryable.ToList()
                    };

                    result.Add(reviewer);
                }
            });

            return result;
        }

        public override async Task<IQueryable<Submission>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(x => x.Conflicts)
                .Include(x => x.Clones)
                .ThenInclude(x => x.Reviews);
        }

        public async Task<object> UpdateStatusRequestForCameraReady(Guid submissionId, bool status)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.Submissions.Any(s => s.Id == submissionId))
                {
                    dbContext.Submissions.Where(s => s.Id == submissionId)
                        .ExecuteUpdate(e => e.SetProperty(x => x.IsRequestedForCameraReady, x => status));
                    Submission queryResult = dbContext.Submissions.Where(s => s.Id == submissionId).First();
                    return new
                    {
                        submissionId = queryResult.Id,
                        status = queryResult.IsRequestedForCameraReady
                    };
                }
                else
                {
                    throw new Exception("SubmissionId does not exist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[ERROR][UpdateStatusRequestForCameraReady] " + ex.Message);
            }
        }

        public async Task<object> UpdateStatusRequestForAllCameraReady(Guid conferenceId, bool status)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.Conferences.Any(c => c.Id == conferenceId))
                {
                    //Get List TrackId by ConferenceId
                    List<Guid> trackIds = dbContext.Tracks.Where(t => t.ConferenceId == conferenceId).Select(a => a.Id).ToList();
                    
                    //Get List SubmissionId by list TrackId
                    List<Guid> submissionIds = new();
                    trackIds.ForEach(tid =>
                    {
                        List<Guid> queryResult = dbContext.Submissions.Where(s => s.TrackId == tid).Select(b => b.Id).ToList();
                        submissionIds.AddRange(queryResult);
                    });

                    //Update Status by List SubmissionId
                    List<object> result = new();
                    submissionIds.ForEach(sid =>
                    {
                        var updateStatus = UpdateStatusRequestForCameraReady(sid, status).Result;
                        result.Add(updateStatus);
                    });

                    //Get return data
                    /* Return All Data of Submission
                    List<Submission> result = new();
                    trackIds.ForEach(tid =>
                    {
                        List<Submission> queryResult = dbContext.Submissions.Where(s => s.TrackId == tid).ToList();
                        result.AddRange(queryResult);
                    });
                    */
                    return result;
                }
                else
                {
                    throw new Exception("ConferenceId does not exist");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[ERROR][UpdateStatusRequestForAllCameraReady] " + ex.Message);
            }
        }
    }
}
