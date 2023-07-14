using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Volo.Abp.Identity;
using Volo.Abp;
using System.Text.Json;
using static Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement.SubmissionRepository;
using Microsoft.Data.SqlClient;
using System.Data;
using Volo.Abp.Timing;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ReviewerRepository : EfCoreRepository<PublicCoreflowDbContext, Reviewer, Guid>, IReviewerRepository
    {
        private readonly IClock _clock;

        private const string Reviewer = "Reviewer";

        public ReviewerRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IClock clock) : base(dbContextProvider)
        {
            _clock = clock;
        }

        public async Task<Reviewer?> FindAsync(Guid accountId, Guid conferenceId, Guid trackId)
        {
            var dbContext = await GetDbContextAsync();

            var reviewerQueryable = (from r in dbContext.Set<ConferenceRole>() select r)
                                    .Where(x => x.Name.Equals(Reviewer));

            var conferenceAccountQueryable = (from ca in dbContext.Set<ConferenceAccount>() select ca)
                                                .Where(x => x.AccountId == accountId && x.ConferenceId == conferenceId);

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>()
                                      join ca in conferenceAccountQueryable on i.ConferenceAccountId equals ca.Id
                                      join r in reviewerQueryable on i.ConferenceRoleId equals r.Id
                                      select i)
                                      .Where(x => x.TrackId == trackId);

            var incumbent = await incumbentQueryable.FirstOrDefaultAsync();

            if (incumbent != null)
            {
                var reviewer = await dbContext.Set<Reviewer>().FindAsync(incumbent.Id);

                return reviewer;
            }

            return null;
        }

        public async Task<Reviewer?> FindAsync(Guid id, Guid trackId)
        {
            var dbContext = await GetDbContextAsync();

            var reviewerQueryable = (from r in dbContext.Set<ConferenceRole>() select r)
                                    .Where(x => x.Name.Equals(Reviewer));

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>()
                                      join r in reviewerQueryable on i.ConferenceRoleId equals r.Id
                                      select i)
                                      .Where(x => x.Id == id && x.TrackId == trackId);

            var incumbent = await incumbentQueryable.FirstOrDefaultAsync();

            if (incumbent != null)
            {
                var reviewer = await dbContext.Set<Reviewer>().FindAsync(incumbent.Id);

                return reviewer;
            }

            return null;
        }

        public async Task<Reviewer?> UpdateReviewerQuota(Guid accountId, Guid conferenceId, Guid trackId, int? quota)
        {
            var dbContext = await GetDbContextAsync();

            var reviewerQueryable = (from r in dbContext.Set<ConferenceRole>() select r)
                                    .Where(x => x.Name.Equals(Reviewer));

            var conferenceAccountQueryable = (from ca in dbContext.Set<ConferenceAccount>() select ca)
                                                .Where(x => x.AccountId == accountId && x.ConferenceId == conferenceId);

            var incumbentQueryable = (from i in dbContext.Set<Incumbent>()
                                      join ca in conferenceAccountQueryable on i.ConferenceAccountId equals ca.Id
                                      join r in reviewerQueryable on i.ConferenceRoleId equals r.Id
                                      select i)
                                      .Where(x => x.TrackId == trackId);

            var incumbent = await incumbentQueryable.FirstOrDefaultAsync();

            if(incumbent != null)
            {
                var reviewer = await dbContext.Set<Reviewer>().FindAsync(incumbent.Id);
                if(reviewer != null)
                {
                    reviewer.Quota = quota;
                    dbContext.SaveChanges();
                } 

                return reviewer;
            }
            
            return null;
        }

        public override async Task<IQueryable<Reviewer>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(x => x.SubjectAreas);
        }

        private double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0 + k);
        }

        private double CalculateRelevance(
            List<SelectedSubjectAreaBriefInfo> submissionSubjectAreas,
            List<SelectedSubjectAreaBriefInfo> reviewerSubjectAreas,
            SubjectAreaRelevanceCoefficients formula)
        {
            var subSaList = new List<SubjectAreaRow>();
            var revSaList = new List<SubjectAreaRow>();

            submissionSubjectAreas.ForEach(x =>
            {
                subSaList.Add(new SubjectAreaRow
                {
                    Id = x.SubjectAreaId,
                    Name = x.SubjectAreaName ?? string.Empty,
                    IsPrimary = x.IsPrimary
                });
            });

            reviewerSubjectAreas.ForEach(x =>
            {
                revSaList.Add(new SubjectAreaRow
                {
                    Id = x.SubjectAreaId,
                    Name = x.SubjectAreaName ?? string.Empty,
                    IsPrimary = x.IsPrimary
                });
            });

            var commonSaList = subSaList.Intersect(revSaList);

            var ppcoef = commonSaList.Any(x => x.IsPrimary) ? 1 : 0;

            var primarySubSa = submissionSubjectAreas.FirstOrDefault(x => x.IsPrimary);
            var spcoef = reviewerSubjectAreas.Any(x => !x.IsPrimary && x.SubjectAreaId == primarySubSa?.SubjectAreaId) ? 1 : 0;

            var primaryRevSa = reviewerSubjectAreas.FirstOrDefault(x => x.IsPrimary);
            var pscoef = submissionSubjectAreas.Any(x => !x.IsPrimary && x.SubjectAreaId == primaryRevSa?.SubjectAreaId) ? 1 : 0;

            var commonSecondarySaList = commonSaList.ToList();
            commonSecondarySaList.RemoveAll(x => x.IsPrimary);

            var sscoef = commonSecondarySaList.Count;

            var temp = Sigmoid(sscoef);

            if (formula.pp != null && formula.sp != null && formula.ps != null && formula.ss != null)
            {
                return Math.Round((double)(formula.pp * ppcoef + formula.sp * spcoef + formula.ps * pscoef
                    + formula.ss * 2 * (Sigmoid(sscoef) - 0.5)), SubmissionConsts.NumberOfRelevanceScoreDigits);
            }

            return 0;
        }

        public async Task<int> GetCountReviewerAggregation(Guid accountId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();

            var reviewerRole = dbContext.Set<ConferenceRole>().FirstOrDefault(x => x.Name.Equals(Reviewer));
            if (reviewerRole == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceRoleReviewerNotFound);
            }

            // Lay danh sach cac incumbent account, conference, tat ca cac track, role reviewer
            var incumbentQueryable = (from ca in dbContext.Set<ConferenceAccount>()
                                      join c in dbContext.Set<Conference>() on ca.ConferenceId equals c.Id
                                      join a in dbContext.Set<IdentityUser>() on ca.AccountId equals a.Id
                                      join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                                      where i.ConferenceRoleId == reviewerRole.Id && c.Id == conferenceId
                                      && a.Id == accountId
                                      select i);

            // suy ra danh sach reviewer trong bang reviewr
            var reviewerQueryable = (from r in dbContext.Set<Reviewer>()
                                     join i in incumbentQueryable on r.Id equals i.Id
                                     select r);

            // dung danh sach nay lay cac review assignment co trang thai isActive
            var reviewAssignmentQueryable = (from ra in dbContext.Set<ReviewAssignment>()
                                             join r in reviewerQueryable on ra.ReviewerId equals r.Id
                                             join sc in dbContext.Set<SubmissionClone>() on ra.SubmissionCloneId equals sc.Id
                                             where sc.IsLast && ra.IsActive
                                             select new
                                             {
                                                 ReviewerId = r.Id,
                                                 ReviewAssignmentId = ra.Id,
                                                 SubmissionId = sc.SubmissionId
                                             });

            // => danh dach cac clone submission, luot lay nhung con IsLast
            // ===> danh sach cac submission

            return (from s in dbContext.Set<Submission>()
                                  join ra in reviewAssignmentQueryable on s.Id equals ra.SubmissionId
                                  join t in dbContext.Set<Track>() on s.TrackId equals t.Id
                                  select new SubmissionWithFacts
                                  {
                                      ReviewAssignmentId = ra.ReviewAssignmentId,
                                      SubmissionId = s.Id,
                                      SubmissionTitle = s.Title,
                                      TrackId = s.TrackId,
                                      TrackName = t.Name,
                                      ReviewerId = ra.ReviewerId,
                                      SubmissionSubjectAreas = null,
                                      Relevance = 0
                                  }).Count();
        }

        public async Task<List<SubmissionWithFacts>> GetListReviewerAggregation(
            Guid accountId, Guid conferenceId,
            string? sorting = ReviewerConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = ReviewerConsts.DefaultMaxResultCount)
        {
            var dbContext = await GetDbContextAsync();

            var reviewerRole = dbContext.Set<ConferenceRole>().FirstOrDefault(x => x.Name.Equals(Reviewer));
            if(reviewerRole == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceRoleReviewerNotFound);
            }

            // Lay danh sach cac incumbent account, conference, tat ca cac track, role reviewer
            var incumbentQueryable = (from ca in dbContext.Set<ConferenceAccount>()
                                      join c in dbContext.Set<Conference>() on ca.ConferenceId equals c.Id
                                      join a in dbContext.Set<IdentityUser>() on ca.AccountId equals a.Id
                                      join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                                      where i.ConferenceRoleId == reviewerRole.Id && c.Id == conferenceId
                                      && a.Id == accountId
                                      select i);

            // suy ra danh sach reviewer trong bang reviewr
            var reviewerQueryable = (from r in dbContext.Set<Reviewer>()
                                     join i in incumbentQueryable on r.Id equals i.Id
                                     select r);

            // dung danh sach nay lay cac review assignment co trang thai isActive
            var reviewAssignmentQueryable = (from ra in dbContext.Set<ReviewAssignment>()
                                             join r in reviewerQueryable on ra.ReviewerId equals r.Id
                                             join sc in dbContext.Set<SubmissionClone>() on ra.SubmissionCloneId equals sc.Id
                                             where sc.IsLast && ra.IsActive
                                             select new
                                             {
                                                 ReviewerId = r.Id,
                                                 ReviewAssignmentId = ra.Id,
                                                 SubmissionId = sc.SubmissionId
                                             });

            // => danh dach cac clone submission, luot lay nhung con IsLast
            // ===> danh sach cac submission

            var submissionList = (from s in dbContext.Set<Submission>()
                                       join ra in reviewAssignmentQueryable on s.Id equals ra.SubmissionId
                                       join t in dbContext.Set<Track>() on s.TrackId equals t.Id
                                       select new SubmissionWithFacts
                                       {
                                           ReviewAssignmentId = ra.ReviewAssignmentId,
                                           SubmissionId = s.Id,
                                           SubmissionTitle = s.Title,
                                           TrackId = s.TrackId,
                                           TrackName = t.Name,
                                           ReviewerId = ra.ReviewerId,
                                           SubmissionSubjectAreas = null,
                                           Relevance = 0
                                       })
                                       .OrderBy(string.IsNullOrWhiteSpace(sorting) ? ReviewerConsts.DefaultSorting : sorting)
                                       .PageBy(skipCount, maxResultCount)
                                       .ToList();

            submissionList.ForEach(x =>
            {
                // lay danh sach cac submission Subject area
                var submissionSubjectAreaList = (from ssa in dbContext.Set<SubmissionSubjectArea>()
                                                 join sa in dbContext.Set<SubjectArea>() on ssa.SubjectAreaId equals sa.Id
                                                 where ssa.SubmissionId == x.SubmissionId
                                                 select new SelectedSubjectAreaBriefInfo
                                                 {
                                                     SubjectAreaId = ssa.SubjectAreaId,
                                                     SubjectAreaName = sa.Name,
                                                     IsPrimary = ssa.IsPrimary
                                                 }).ToList();
                x.SubmissionSubjectAreas = submissionSubjectAreaList;

                // lay danh sach cac reviewer subject area
                var reviewerSubjectAreaList = (from rsa in dbContext.Set<ReviewerSubjectArea>()
                                               join sa in dbContext.Set<SubjectArea>() on rsa.SubjectAreaId equals sa.Id
                                               where rsa.ReviewerId == x.ReviewerId
                                               select new SelectedSubjectAreaBriefInfo
                                               {
                                                   SubjectAreaId = rsa.SubjectAreaId,
                                                   SubjectAreaName = sa.Name,
                                                   IsPrimary = rsa.IsPrimary
                                               }).ToList();

                // tinh toan relevance score
                var track = dbContext.Set<Track>().Find(x.TrackId);
                if(track == null)
                {
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
                }
                var relevanceFormula = track.SubjectAreaRelevanceCoefficients == null ?
                TrackConsts.DefaultSubjectAreaRelevanceCoefficients :
                JsonSerializer.Deserialize<SubjectAreaRelevanceCoefficients>(track.SubjectAreaRelevanceCoefficients);
                relevanceFormula ??= TrackConsts.DefaultSubjectAreaRelevanceCoefficients;

                x.Relevance = CalculateRelevance(submissionSubjectAreaList, reviewerSubjectAreaList, relevanceFormula);
            });

            // xuat theo danh sach dau cua cac submission
            return submissionList;
        }

        public async Task<List<GetReviewerReviewingInformationAggregationSPO>?> GetReviewerReviewingInformationAggregationAsync(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid accountId,
            string? sorting,
            bool? sortedAsc,
            int skipCount,
            int maxResultCount)
        {
            var dbContext = await GetDbContextAsync();

            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter() {
                    ParameterName = "@UTCNowStr",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 20,
                    Direction = ParameterDirection.Input,
                    Value = _clock.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                },
                new SqlParameter() {
                    ParameterName = "@InclusionText",
                    SqlDbType = SqlDbType.NVarChar,
                    Size = 1024,
                    Direction = ParameterDirection.Input,
                    Value = inclusionText == null || string.IsNullOrWhiteSpace(inclusionText) ? DBNull.Value : inclusionText.Trim()
                },
                new SqlParameter() {
                    ParameterName = "@ConferenceId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    Value = conferenceId
                },
                new SqlParameter() {
                    ParameterName = "@TrackId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    Value = trackId == null ? DBNull.Value : trackId
                },
                new SqlParameter() {
                    ParameterName = "@AccountId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    Value = accountId
                },
                new SqlParameter() {
                    ParameterName = "@Sorting",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 128,
                    Direction = ParameterDirection.Input,
                    Value = sorting == null || string.IsNullOrWhiteSpace(sorting) ? DBNull.Value : sorting.Trim()
                },
                new SqlParameter() {
                    ParameterName = "@SortedAsc",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Input,
                    Value = sortedAsc == null ? DBNull.Value : sortedAsc
                },
                new SqlParameter() {
                    ParameterName = "@SkipCount",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = skipCount
                },
                new SqlParameter() {
                    ParameterName = "@MaxResultCount",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Input,
                    Value = maxResultCount
                }
            };

            var resultList = await dbContext.Set<GetReviewerReviewingInformationAggregationSPO>().FromSqlRaw(@"
                EXECUTE [dbo].GetReviewerReviewingInformationAggregation @UTCNowStr, @InclusionText, @ConferenceId, @TrackId, @AccountId, @Sorting, @SortedAsc, @SkipCount, @MaxResultCount", sqlParameters.ToArray()).ToListAsync();

            if (resultList.Count > 0)
            {
                return resultList;
            }

            return null;
        }
    }
}
