using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public async Task<object> GetNumberOfSubmission(Guid trackId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Submissions.Any(s => s.TrackId == trackId))
                {
                    throw new Exception("There Is no Submission for TrackID=" + trackId);
                }
                var totalSubmission = dbContext.Submissions.Where(s => s.TrackId == trackId).Count();
                return new
                {
                    numOfSubmission = totalSubmission
                };
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][GetNumberOfSubmission] " + ex.Message, ex);
            }
        }

        public async Task<object> GetNumOfSubmissionAndEmailWithAllAuthor(SubmissionWithEmailRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Tracks.Any(t => t.Id == request.TrackId))
                    throw new Exception("TrackId not existing");
                if (!request.PaperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.PaperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.TrackId && s.StatusId == ps);
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        statusName = statusName,
                        numberSubmission = submissionOfOneStatus,
                        numberEmailSend = numOfAllAuthor
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
                if (!dbContext.Tracks.Any(t => t.Id == request.TrackId))
                    throw new Exception("TrackId not existing");
                if (!request.PaperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.PaperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.TrackId && s.StatusId == ps);
                    var numOfPrimaryAuthor = submissions.SelectMany(ss => ss.Authors).Where(au => au.IsPrimaryContact).Count();
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        statusName = statusName,
                        numberSubmission = submissionOfOneStatus,
                        numberEmailSend = numOfPrimaryAuthor,
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

        public class SubjectAreaRow : SubjectAreaBriefInfo, IEquatable<SubjectAreaRow>
        {
            public bool IsPrimary { get; set; }

            public bool Equals(SubjectAreaRow? other)
            {
                if (other is null)
                    return false;
                return this.Id == other.Id && this.IsPrimary == other.IsPrimary;
            }

            public override bool Equals(object? obj) => Equals(obj as SubjectAreaRow);
            public override int GetHashCode() => (Id, Name, IsPrimary).GetHashCode();
        }

        private double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0 + k);
        }

        private double CalculateRelevance(
            List<SubmissionSubjectAreaBriefInfo> submissionSubjectAreas,
            List<ReviewerSubjectAreaBriefInfo> reviewerSubjectAreas,
            SubjectAreaRelevanceCoefficients formula)
        {
            var subSaList = new List<SubjectAreaRow>();
            var revSaList = new List<SubjectAreaRow>();

            submissionSubjectAreas.ForEach(x =>
            {
                subSaList.Add(new SubjectAreaRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsPrimary = x.IsPrimary
                });
            });

            reviewerSubjectAreas.ForEach(x =>
            {
                revSaList.Add(new SubjectAreaRow
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsPrimary = x.IsPrimary
                });
            });

            var commonSaList = subSaList.Intersect(revSaList);

            var ppcoef = commonSaList.Any(x => x.IsPrimary) ? 1 : 0;

            var primarySubSa = submissionSubjectAreas.First(x => x.IsPrimary);
            var spcoef = reviewerSubjectAreas.Any(x => !x.IsPrimary && x.Id == primarySubSa.Id) ? 1 : 0;

            var primaryRevSa = reviewerSubjectAreas.First(x => x.IsPrimary);
            var pscoef = submissionSubjectAreas.Any(x => !x.IsPrimary && x.Id == primaryRevSa.Id) ? 1 : 0;

            var commonSecondarySaList = commonSaList.ToList();
            commonSecondarySaList.RemoveAll(x => x.IsPrimary);

            var sscoef = commonSecondarySaList.Count;

            if (formula.pp != null && formula.sp != null && formula.ps != null && formula.ss != null)
            {
                return Math.Round((double)(formula.pp * ppcoef + formula.sp * spcoef + formula.ps * pscoef
                    + formula.ss * Sigmoid(sscoef)), SubmissionConsts.NumberOfRelevanceScoreDigits);
            }

            return 0;
        }

        public async Task<List<ReviewerWithFacts>> GetListReviewerWithFacts(Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            var submission = await FindAsync(submissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            var track = await dbContext.Tracks.FindAsync(submission.TrackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var relevanceFormula = track.SubjectAreaRelevanceCoefficients == null ?
                TrackConsts.DefaultSubjectAreaRelevanceCoefficients :
                JsonSerializer.Deserialize<SubjectAreaRelevanceCoefficients>(track.SubjectAreaRelevanceCoefficients);

            var submissionSubjectAreaQueryable = (from ssa in dbContext.Set<SubmissionSubjectArea>()
                                                  join sa in dbContext.Set<SubjectArea>() on ssa.SubjectAreaId equals sa.Id
                                                  select new SubmissionSubjectAreaBriefInfo
                                                  {
                                                      SubmissionId = ssa.SubmissionId,
                                                      Id = ssa.SubjectAreaId,
                                                      Name = sa.Name,
                                                      IsPrimary = ssa.IsPrimary
                                                  })
                                                  .Where(y => y.SubmissionId == submissionId);

            var submissionSubjectAreaList = submissionSubjectAreaQueryable.ToList();

            List<ReviewerWithFacts> result = new List<ReviewerWithFacts>();

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

                if (user != null)
                {
                    var submissionConflictQueryable = (from c in dbContext.Set<Conflict>()
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

                    var reviewerConflictQueryable = (from c in dbContext.Set<Conflict>()
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
                                         && y.IsDefinedByReviewer);

                    var reviewerSubjectAreaQueryable = (from rsa in dbContext.Set<ReviewerSubjectArea>()
                                                        join sa in dbContext.Set<SubjectArea>() on rsa.SubjectAreaId equals sa.Id
                                                        select new ReviewerSubjectAreaBriefInfo
                                                        {
                                                            ReviewerId = rsa.ReviewerId,
                                                            Id = rsa.SubjectAreaId,
                                                            Name = sa.Name,
                                                            IsPrimary = rsa.IsPrimary
                                                        })
                                                        .Where(y => y.ReviewerId == x.ReviewerId);

                    var reviewerSubjectAreaList = reviewerSubjectAreaQueryable.ToList();

                    // Calculate Relevance Score
                    var relevanceScore = CalculateRelevance(submissionSubjectAreaList,
                        reviewerSubjectAreaList, relevanceFormula ?? TrackConsts.DefaultSubjectAreaRelevanceCoefficients);

                    // Get Quota
                    var foundReviewer = (from r in dbContext.Set<Reviewer>() select r)
                                            .Where(y => y.Id == x.ReviewerId).FirstOrDefault();
                    var quota = foundReviewer == null ? null : foundReviewer.Quota;

                    // Get NumberOfAssignments
                    var trackSubmissionQueryable = (from s in dbContext.Set<Submission>() select s)
                                                    .Where(y => y.TrackId == submission.TrackId);

                    var reviewAssignmentQueryable = (from sc in dbContext.Set<SubmissionClone>()
                                                     join s in trackSubmissionQueryable on sc.SubmissionId equals s.Id
                                                     select sc)
                                                    .Where(y => y.SubmissionId == submission.Id);
                    // Bo sung isLast

                    // Get IsAssigned
                    var lastSubmissionClone = (from sc in dbContext.Set<SubmissionClone>()
                                               select sc)
                                                        .Where(y => y.SubmissionId == submission.Id)
                                                        .OrderBy(y => y.CreationTime)
                                                        .LastOrDefault();

                    var isAssigned = false;
                    if (lastSubmissionClone != null && foundReviewer != null)
                    {
                        isAssigned = (from ra in dbContext.Set<ReviewAssignment>()
                                      select ra)
                                      .Where(y => y.SubmissionCloneId == lastSubmissionClone.Id
                                                  && y.ReviewerId == foundReviewer.Id
                                                  && y.IsActive).ToList().Count > 0;
                    }

                    ReviewerWithFacts reviewer = new ReviewerWithFacts
                    {
                        ReviewerId = x.ReviewerId,
                        FirstName = user.Name,
                        MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName),
                        LastName = user.Surname,
                        Email = user.Email,
                        Organization = user.GetProperty<string?>(AccountConsts.OrganizationPropertyName),
                        SubmissionConflicts = submissionConflictQueryable.ToList(),
                        ReviewerConflicts = reviewerConflictQueryable.ToList(),
                        ReviewerSubjectAreas = reviewerSubjectAreaList,
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
    }
}
