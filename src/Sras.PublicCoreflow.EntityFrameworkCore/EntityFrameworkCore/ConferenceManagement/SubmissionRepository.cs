using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class SubmissionRepository : EfCoreRepository<PublicCoreflowDbContext, Submission, Guid>, ISubmissionRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly string HostBlobPrefix = "host";

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

            var submission = await base.FindAsync(submissionId);
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

            var submission = await base.FindAsync(submissionId);
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

            var primarySubSa = submissionSubjectAreas.FirstOrDefault(x => x.IsPrimary);
            var spcoef = reviewerSubjectAreas.Any(x => !x.IsPrimary && x.Id == primarySubSa?.Id) ? 1 : 0;

            var primaryRevSa = reviewerSubjectAreas.FirstOrDefault(x => x.IsPrimary);
            var pscoef = submissionSubjectAreas.Any(x => !x.IsPrimary && x.Id == primaryRevSa?.Id) ? 1 : 0;

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

        public async Task<SubmissionReviewerAssignmentSuggestion> GetSubmissionReviewerAssignmentSuggestionAsync(Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            var submission = await base.FindAsync(submissionId);
            if (submission == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.SubmissionNotFound);

            var track = await dbContext.Tracks.FindAsync(submission.TrackId);
            if (track == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

            var result = new SubmissionReviewerAssignmentSuggestion();

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

            List<ReviewerWithFacts> reviewers = new List<ReviewerWithFacts>();

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

                    var submissionCloneQueryable = (from sc in dbContext.Set<SubmissionClone>()
                                                     join s in trackSubmissionQueryable on sc.SubmissionId equals s.Id
                                                     select sc)
                                                    .Where(y => y.IsLast);
                    var reviewAssignmentQueryable = (from ra in dbContext.Set<ReviewAssignment>()
                                                     join sc in submissionCloneQueryable on ra.SubmissionCloneId equals sc.Id
                                                     select ra)
                                                     .Where(y => y.ReviewerId == x.ReviewerId);
                    var numberOfAssignments = reviewAssignmentQueryable.Count();

                    // Get IsAssigned
                    var lastSubmissionClone = (from sc in dbContext.Set<SubmissionClone>()
                                               select sc)
                                                        .Where(y => y.SubmissionId == submission.Id)
                                                        .OrderBy(y => y.CreationTime)
                                                        .LastOrDefault();

                    var isAssigned = false;
                    if (lastSubmissionClone != null)
                    {
                        isAssigned = (from ra in dbContext.Set<ReviewAssignment>()
                                      select ra)
                                      .Where(y => y.SubmissionCloneId == lastSubmissionClone.Id
                                                  && y.ReviewerId == x.ReviewerId
                                                  && y.IsActive).ToList().Count > 0;
                    }

                    var sortingFactor = 2;
                    if(!submissionConflictQueryable.Any() && !reviewerConflictQueryable.Any())
                    {
                        if(quota != null && quota - numberOfAssignments <= 0)
                        {
                            sortingFactor = 1;
                        }
                        else
                        {
                            sortingFactor = 0;
                        }
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
                        Relevance = relevanceScore,
                        Quota = quota,
                        NumberOfAssignments = numberOfAssignments,
                        IsAssigned = isAssigned,
                        SortingFactor = sortingFactor
                    };

                    reviewers.Add(reviewer);
                }
            });

            result.TrackId = track.Id;
            result.TrackName = track.Name;
            result.SubmissionId = submission.Id;
            result.SubmissionTitle = submission.Title;
            result.SubmissionSubjectAreas = submissionSubjectAreaList;
            result.Reviewers = reviewers.OrderBy(x => x.SortingFactor)
                .ThenBy(x => -x.Relevance)
                .ToList();

            return result;
        }

        private class SubmissionProjection
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public string Abstract { get; set; } = string.Empty;
            public Guid TrackId { get; set; }
            public Guid StatusId { get; set; }
            public bool IsRequestedForCameraReady { get; set; }
            public bool IsRequestedForPresentation { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime? LastModificationTime { get; set; }
        }

        public async Task<List<SubmissionAggregation>> GetListSubmissionAggregation(
            Guid conferenceId,
            Guid? trackId = null,
            string? sorting = SubmissionConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = SubmissionConsts.DefaultMaxResultCount)
        {
            var dbContext = await GetDbContextAsync();

            if (!await dbContext.Set<Conference>().AnyAsync(x => x.Id == conferenceId))
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            IQueryable< SubmissionProjection> submissionQueryable;
            if (trackId != null)
            {
                if (!await dbContext.Set<Track>().AnyAsync(x => x.Id == trackId))
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

                submissionQueryable = (from s in dbContext.Set<Submission>()
                                    where s.TrackId == trackId
                                    select new SubmissionProjection
                                    {
                                        Id = s.Id,
                                        Title = s.Title,
                                        Abstract = s.Abstract,
                                        TrackId = s.TrackId,
                                        StatusId = s.StatusId,
                                        IsRequestedForCameraReady = s.IsRequestedForCameraReady,
                                        IsRequestedForPresentation = s.IsRequestedForPresentation,
                                        CreationTime = s.CreationTime,
                                        LastModificationTime = s.LastModificationTime
                                    })
                                    .OrderByDescending(x => x.LastModificationTime ??  DateTime.MinValue)
                                    .ThenByDescending(x => x.CreationTime)
                                    .PageBy(skipCount, maxResultCount);
            }
            else
            {
                var conferenceTrackQueryable = (from t in dbContext.Set<Track>()
                                                where t.ConferenceId == conferenceId
                                                select new
                                                {
                                                    Id = t.Id
                                                });

                submissionQueryable = (from s in dbContext.Set<Submission>()
                                       join t in conferenceTrackQueryable on s.TrackId equals t.Id
                                       select new SubmissionProjection
                                       {
                                           Id = s.Id,
                                           Title = s.Title,
                                           Abstract = s.Abstract,
                                           TrackId = s.TrackId,
                                           StatusId = s.StatusId,
                                           IsRequestedForCameraReady = s.IsRequestedForCameraReady,
                                           IsRequestedForPresentation = s.IsRequestedForPresentation,
                                           CreationTime = s.CreationTime,
                                           LastModificationTime = s.LastModificationTime
                                       })
                                    .OrderByDescending(x => x.LastModificationTime ?? DateTime.MinValue)
                                    .ThenByDescending(x => x.CreationTime)
                                    .PageBy(skipCount, maxResultCount);
            }

            var submissionList = await submissionQueryable.ToListAsync();

            List<SubmissionAggregation> result = new List<SubmissionAggregation>();

            submissionList.ForEach(x =>
            {
                SubmissionAggregation submission = new SubmissionAggregation();

                submission.SubmissionId = x.Id;
                submission.SubmissionTitle = x.Title;
                submission.SubmissionAbstract = x.Abstract;

                var authorList = (from a in dbContext.Set<Author>()
                                  where a.SubmissionId == x.Id
                                  select new
                                  {
                                      Id = a.Id,
                                      IsPrimaryContact = a.IsPrimaryContact,
                                      ParticipantId = a.ParticipantId,
                                  }).ToList();
                List<AuthorBriefInfo> authors = new List<AuthorBriefInfo>();
                int numberOfUnregisteredAuthors = 0;
                authorList.ForEach(y =>
                {
                    var participant = dbContext.Set<Participant>().Find(y.ParticipantId);
                    var author = new AuthorBriefInfo();
                    author.Id = y.Id;
                    author.IsPrimaryContact = y.IsPrimaryContact;
                    author.ParticipantId = y.ParticipantId;

                    if (participant != null)
                    {
                        if (participant.AccountId != null)
                        {
                            var user = dbContext.Set<IdentityUser>().Find(participant.AccountId);
                            if (user != null)
                            {
                                author.FirstName = user.Name;
                                author.MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName);
                                author.LastName = user.Surname;
                                author.Email = user.Email;

                                authors.Add(author);
                            }
                        }
                        else if (participant.OutsiderId != null)
                        {
                            var outsider = dbContext.Set<Outsider>()
                                            .Where(o => o.Id == participant.OutsiderId)
                                            .Select(o => new {o.FirstName, o.MiddleName, o.LastName, o.Email}).SingleOrDefault();
                            if (outsider != null)
                            {
                                author.FirstName = outsider.FirstName;
                                author.MiddleName = outsider.MiddleName;
                                author.LastName = outsider.LastName;
                                author.Email = outsider.Email;

                                authors.Add(author);

                                numberOfUnregisteredAuthors++;
                            }
                        }
                    }
                });
                submission.Authors = authors;
                submission.NumberOfUnregisteredAuthors = numberOfUnregisteredAuthors;

                var path = HostBlobPrefix + "/" + x.Id.ToString();
                if (Directory.Exists(path))
                {
                    submission.NumberOfSubmissionFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Length;
                }
                else
                {
                    submission.NumberOfSubmissionFiles = 0;
                }

                var submissionSubjectAreaQueryable = (from ssa in dbContext.Set<SubmissionSubjectArea>() select new
                                                    {
                                                        SubmissionId = ssa.SubmissionId,
                                                        SubjectAreaId = ssa.SubjectAreaId,
                                                        IsPrimary = ssa.IsPrimary
                                                    })
                                                     .Where(y => y.SubmissionId == x.Id);
                var submissionSubjectAreas = (from ssa in submissionSubjectAreaQueryable
                                              join sa in dbContext.Set<SubjectArea>() on ssa.SubjectAreaId equals sa.Id
                                              select new SelectedSubjectAreaBriefInfo
                                              {
                                                  SubjectAreaId = ssa.SubjectAreaId,
                                                  SubjectAreaName = sa.Name,
                                                  IsPrimary = ssa.IsPrimary
                                              }).ToList();
                submission.SubmissionSubjectAreas = submissionSubjectAreas;

                var submissionTrack = dbContext.Set<Track>().Where(t => t.Id == x.TrackId).Select(t => new {t.Id, t.Name, t.IsDefault}).SingleOrDefault();
                if (submissionTrack != null)
                {
                    var t = new TrackBriefInfo(submissionTrack.Id, submissionTrack.Name);
                    t.IsDefault = submissionTrack.IsDefault;
                    submission.Track = t;
                }

                // Temporary ChairNote is null
                submission.ChairNoteId = null;

                var conflictList = (from c in dbContext.Set<Conflict>()
                                    select new { IncumbentId = c.IncumbentId, SubmissionId = c.SubmissionId })
                                    .Where(y => y.SubmissionId == x.Id).ToList();
                var conflictedReviewerList = new List<Guid>();
                conflictList.ForEach(y =>
                {
                    if (!conflictedReviewerList.Any(r => r == y.IncumbentId))
                    {
                        var incumbent = dbContext.Set<Incumbent>().Find(y.IncumbentId);
                        if (incumbent != null)
                        {
                            conflictedReviewerList.Add(y.IncumbentId);
                        }
                    }
                });
                submission.NumberOfConflicts = conflictedReviewerList.Count;

                // Temporary NumberOfDisputedConflicts = 0
                submission.NumberOfDisputedConflicts = 0;

                var lastCloneSubmission = (from c in dbContext.Set<SubmissionClone>()
                                           select new
                                           {
                                               Id = c.Id,
                                               SubmissionId = c.SubmissionId,
                                               IsLast = c.IsLast,
                                           })
                                           .Where(y => y.SubmissionId == x.Id && y.IsLast)
                                           .SingleOrDefault();

                var reviewerList = (from r in dbContext.Set<Reviewer>()
                                    join ra in dbContext.Set<ReviewAssignment>() on r.Id equals ra.ReviewerId
                                    join i in dbContext.Set<Incumbent>() on r.Id equals i.Id
                                    select ra)
                                         .Where(y => y.IsActive && lastCloneSubmission != null && y.SubmissionCloneId == lastCloneSubmission.Id)
                                         .ToList();
                var reviewers = new List<ReviewerBriefInfo>();

                int numberOfCompletedReviews = 0;
                reviewerList.ForEach(y =>
                {
                    var found = (from ca in dbContext.Set<ConferenceAccount>()
                                 join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                                 join u in dbContext.Set<IdentityUser>() on ca.AccountId equals u.Id
                                 select new
                                 {
                                     ReviewerId = i.Id,
                                     AccountId = u.Id,
                                 })
                                 .Where(r => r.ReviewerId == y.ReviewerId)
                                 .SingleOrDefault();

                    var user = dbContext.Set<IdentityUser>().Find(found?.AccountId);
                    if (found != null && user != null)
                    {
                        reviewers.Add(new ReviewerBriefInfo
                        {
                            ReviewerId = found.ReviewerId,
                            FirstName = user.Name,
                            MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName),
                            LastName = user.Surname,
                            Email = user.Email,
                            Organization = user.GetProperty<string?>(AccountConsts.OrganizationPropertyName)
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(y.Review))
                    {
                        numberOfCompletedReviews++;
                    }
                });

                submission.Reviewers = reviewers;
                submission.NumberOfAssignment = reviewers.Count;
                submission.NumberOfCompletedReviews = numberOfCompletedReviews;

                var status = dbContext.Set<PaperStatus>().Where(y => y.Id == x.StatusId).Select(y => new {y.Name}).SingleOrDefault();
                submission.Status = status?.Name;

                var revision = dbContext.Set<Revision>().Find(lastCloneSubmission?.Id);
                submission.IsRevisionSubmitted = revision == null ? false : !revision.RootFilePath.IsNullOrEmpty();

                submission.IsRequestedForCameraReady = x.IsRequestedForCameraReady;

                // Temporary IsCameraReadySubmitted = false
                submission.IsCameraReadySubmitted = false;

                submission.IsRequestedForPresentation = x.IsRequestedForPresentation;

                result.Add(submission);
            });

            return await Task.FromResult(result);
        }

        public async Task<List<SubmissionAggregation>> GetListSubmissionAggregation_v2(
            Guid conferenceId,
            Guid? trackId = null,
            string? sorting = SubmissionConsts.DefaultSorting,
            int skipCount = 0,
            int maxResultCount = SubmissionConsts.DefaultMaxResultCount)
        {
            var dbContext = await GetDbContextAsync();

            if (!await dbContext.Set<Conference>().AnyAsync(x => x.Id == conferenceId))
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);

            IQueryable<SubmissionProjection> submissionQueryable;
            if (trackId != null)
            {
                if (!await dbContext.Set<Track>().AnyAsync(x => x.Id == trackId))
                    throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);

                submissionQueryable = dbContext.Set<Submission>()
                    .Where(s => s.TrackId == trackId)
                    .Select(s => new SubmissionProjection
                    {
            Id = s.Id,
            Title = s.Title,
            Abstract = s.Abstract,
            TrackId = s.TrackId,
                StatusId = s.StatusId,
            IsRequestedForCameraReady = s.IsRequestedForCameraReady,
            IsRequestedForPresentation = s.IsRequestedForPresentation,
            CreationTime = s.CreationTime,
            LastModificationTime = s.LastModificationTime
                    })
            .OrderByDescending(x => x.LastModificationTime ?? DateTime.MinValue)
            .ThenByDescending(x => x.CreationTime)
            .Skip(skipCount)
            .Take(maxResultCount);
            }
            else
            {
                var conferenceTrackQueryable = dbContext.Set<Track>()
    .Where(t => t.ConferenceId == conferenceId)
    .Select(t => new
    {
        Id = t.Id
    });

                submissionQueryable = dbContext.Set<Submission>()
    .Join(conferenceTrackQueryable,
        s => s.TrackId,
        t => t.Id,
        (s, t) => new SubmissionProjection
        {
            Id = s.Id,
            Title = s.Title,
            Abstract = s.Abstract,
            TrackId = s.TrackId,
            StatusId = s.StatusId,
            IsRequestedForCameraReady = s.IsRequestedForCameraReady,
            IsRequestedForPresentation = s.IsRequestedForPresentation,
            CreationTime = s.CreationTime,
            LastModificationTime = s.LastModificationTime
        })
    .OrderByDescending(x => x.LastModificationTime ?? DateTime.MinValue)
    .ThenByDescending(x => x.CreationTime)
    .Skip(skipCount)
    .Take(maxResultCount);
            }

            var submissionList = await submissionQueryable.ToListAsync();

            List<SubmissionAggregation> result = new List<SubmissionAggregation>();

            submissionList.ForEach(x =>
            {
                SubmissionAggregation submission = new SubmissionAggregation();

                submission.SubmissionId = x.Id;
                submission.SubmissionTitle = x.Title;
                submission.SubmissionAbstract = x.Abstract;

                var authorList = dbContext.Set<Author>()
     .Where(a => a.SubmissionId == x.Id)
     .Select(a => new
     {
         Id = a.Id,
         IsPrimaryContact = a.IsPrimaryContact,
         ParticipantId = a.ParticipantId
     })
     .ToList();
                List<AuthorBriefInfo> authors = new List<AuthorBriefInfo>();
                int numberOfUnregisteredAuthors = 0;
                authorList.ForEach(y =>
                {
                    var participant = dbContext.Set<Participant>().Find(y.ParticipantId);
                    var author = new AuthorBriefInfo();
                    author.Id = y.Id;
                    author.IsPrimaryContact = y.IsPrimaryContact;
                    author.ParticipantId = y.ParticipantId;

                    if (participant != null)
                    {
                        if (participant.AccountId != null)
                        {
                            var user = dbContext.Set<IdentityUser>().Find(participant.AccountId);
                            if (user != null)
                            {
                                author.FirstName = user.Name;
                                author.MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName);
                                author.LastName = user.Surname;
                                author.Email = user.Email;

                                authors.Add(author);
                            }
                        }
                        else if (participant.OutsiderId != null)
                        {
                            var outsider = dbContext.Set<Outsider>()
                                            .Where(o => o.Id == participant.OutsiderId)
                                            .Select(o => new { o.FirstName, o.MiddleName, o.LastName, o.Email }).SingleOrDefault();
                            if (outsider != null)
                            {
                                author.FirstName = outsider.FirstName;
                                author.MiddleName = outsider.MiddleName;
                                author.LastName = outsider.LastName;
                                author.Email = outsider.Email;

                                authors.Add(author);

                                numberOfUnregisteredAuthors++;
                            }
                        }
                    }
                });
                submission.Authors = authors;
                submission.NumberOfUnregisteredAuthors = numberOfUnregisteredAuthors;

                var path = HostBlobPrefix + "/" + x.Id.ToString();
                if (Directory.Exists(path))
                {
                    submission.NumberOfSubmissionFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Length;
                }
                else
                {
                    submission.NumberOfSubmissionFiles = 0;
                }

                var submissionSubjectAreaQueryable = dbContext.Set<SubmissionSubjectArea>()
    .Select(ssa => new
    {
        SubmissionId = ssa.SubmissionId,
        SubjectAreaId = ssa.SubjectAreaId,
        IsPrimary = ssa.IsPrimary
    })
    .Where(y => y.SubmissionId == x.Id);
                var submissionSubjectAreas = submissionSubjectAreaQueryable
    .Join(dbContext.Set<SubjectArea>(),
        ssa => ssa.SubjectAreaId,
        sa => sa.Id,
        (ssa, sa) => new SelectedSubjectAreaBriefInfo
        {
            SubjectAreaId = ssa.SubjectAreaId,
            SubjectAreaName = sa.Name,
            IsPrimary = ssa.IsPrimary
        })
    .ToList();
                submission.SubmissionSubjectAreas = submissionSubjectAreas;

                var submissionTrack = dbContext.Set<Track>().Where(t => t.Id == x.TrackId).Select(t => new { t.Id, t.Name, t.IsDefault }).SingleOrDefault();
                if (submissionTrack != null)
                {
                    var t = new TrackBriefInfo(submissionTrack.Id, submissionTrack.Name);
                    t.IsDefault = submissionTrack.IsDefault;
                    submission.Track = t;
                }

                // Temporary ChairNote is null
                submission.ChairNoteId = null;

                var conflictList = dbContext.Set<Conflict>()
    .Select(c => new { IncumbentId = c.IncumbentId, SubmissionId = c.SubmissionId })
    .Where(y => y.SubmissionId == x.Id)
    .ToList();
                var conflictedReviewerList = new List<Guid>();
                conflictList.ForEach(y =>
                {
                    if (!conflictedReviewerList.Any(r => r == y.IncumbentId))
                    {
                        var incumbent = dbContext.Set<Incumbent>().Find(y.IncumbentId);
                        if (incumbent != null)
                        {
                            conflictedReviewerList.Add(y.IncumbentId);
                        }
                    }
                });
                submission.NumberOfConflicts = conflictedReviewerList.Count;

                // Temporary NumberOfDisputedConflicts = 0
                submission.NumberOfDisputedConflicts = 0;

                var lastCloneSubmission = dbContext.Set<SubmissionClone>()
    .Select(c => new
    {
        Id = c.Id,
        SubmissionId = c.SubmissionId,
        IsLast = c.IsLast
    })
    .SingleOrDefault(y => y.SubmissionId == x.Id && y.IsLast);

               var reviewerList = dbContext.Set<Reviewer>()
    .Join(dbContext.Set<ReviewAssignment>(),
        r => r.Id,
        ra => ra.ReviewerId,
        (r, ra) => new { Reviewer = r, ReviewAssignment = ra })
    .Join(dbContext.Set<Incumbent>(),
        combined => combined.Reviewer.Id,
        i => i.Id,
        (combined, i) => new { combined.ReviewAssignment, Incumbent = i })
    .Where(combined => combined.ReviewAssignment.IsActive &&
                       lastCloneSubmission != null &&
                       combined.ReviewAssignment.SubmissionCloneId == lastCloneSubmission.Id)
    .Select(combined => combined.ReviewAssignment)
    .ToList();
                var reviewers = new List<ReviewerBriefInfo>();

                int numberOfCompletedReviews = 0;
                reviewerList.ForEach(y =>
                {
                    var found = (from ca in dbContext.Set<ConferenceAccount>()
                                 join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                                 join u in dbContext.Set<IdentityUser>() on ca.AccountId equals u.Id
                                 select new
                                 {
                                     ReviewerId = i.Id,
                                     AccountId = u.Id,
                                 })
                                 .Where(r => r.ReviewerId == y.ReviewerId)
                                 .SingleOrDefault();

                    var user = dbContext.Set<IdentityUser>().Find(found?.AccountId);
                    if (found != null && user != null)
                    {
                        reviewers.Add(new ReviewerBriefInfo
                        {
                            ReviewerId = found.ReviewerId,
                            FirstName = user.Name,
                            MiddleName = user.GetProperty<string?>(AccountConsts.MiddleNamePropertyName),
                            LastName = user.Surname,
                            Email = user.Email,
                            Organization = user.GetProperty<string?>(AccountConsts.OrganizationPropertyName)
                        });
                    }

                    if (!string.IsNullOrWhiteSpace(y.Review))
                    {
                        numberOfCompletedReviews++;
                    }
                });

                submission.Reviewers = reviewers;
                submission.NumberOfAssignment = reviewers.Count;
                submission.NumberOfCompletedReviews = numberOfCompletedReviews;

                var status = dbContext.Set<PaperStatus>().Where(y => y.Id == x.StatusId).Select(y => new { y.Name }).SingleOrDefault();
                submission.Status = status?.Name;

                var revision = dbContext.Set<Revision>().Find(lastCloneSubmission?.Id);
                submission.IsRevisionSubmitted = revision == null ? false : !revision.RootFilePath.IsNullOrEmpty();

                submission.IsRequestedForCameraReady = x.IsRequestedForCameraReady;

                // Temporary IsCameraReadySubmitted = false
                submission.IsCameraReadySubmitted = false;

                submission.IsRequestedForPresentation = x.IsRequestedForPresentation;

                result.Add(submission);
            });

            return await Task.FromResult(result);
        }

        public async Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid conferenceId, Guid accountId)
        {
            var dbContext = await GetDbContextAsync();

            var account = await dbContext.Set<IdentityUser>().FindAsync(accountId);
            if (account == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.AccountNotFound);
            }

            var conference = await dbContext.Set<Conference>().FindAsync(conferenceId);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            RegistrablePaperTable result = new RegistrablePaperTable();

            result.AccountId = account.Id;
            result.Email = account.Email;

            var participant = await dbContext.Set<Participant>().Where(x => x.AccountId == account.Id).SingleOrDefaultAsync();

            if(participant == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ParticipantNotFound);
            }

            var conferenceSubmissionQueryable = (from s in dbContext.Set<Submission>()
                                                 join t in dbContext.Set<Track>() on s.TrackId equals t.Id
                                                 join a in dbContext.Set<Author>() on s.Id equals a.SubmissionId
                                                 where t.ConferenceId == conference.Id && a.ParticipantId == participant.Id
                                                 select s);

            var registrablePaperList = await (from s in conferenceSubmissionQueryable
                                              join cr in dbContext.Set<CameraReady>() on s.Id equals cr.Id
                                              select s).ToListAsync();

            List<SubmissionBriefInfo> registrablePapers = new List<SubmissionBriefInfo>();

            registrablePaperList.ForEach(x =>
            {
                SubmissionBriefInfo submission = new SubmissionBriefInfo();
                submission.SubmissionId = x.Id;
                submission.SubmissionTitle = x.Title;

                var track = dbContext.Set<Track>().Find(x.TrackId);
                if(track != null)
                {
                    submission.TrackId = track.Id;
                    submission.TrackName = track.Name;
                }

                registrablePapers.Add(submission);
            });

            result.RegistrablePapers = registrablePapers;

            return result;
        }

        public async Task<List<SubmissionAggregationSP>> GetListSubmissionAggregationSP(
            string? inclusionText,
            Guid conferenceId,
            Guid? trackId,
            Guid? statusId,
            int skipCount,
            int maxResultCount
        )
        {
            var dbContext = await GetDbContextAsync();

            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter() {
                    ParameterName = "@InclusionText",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 1024,
                    Direction = ParameterDirection.Input,
                    Value = string.IsNullOrWhiteSpace(inclusionText) || inclusionText == null ? DBNull.Value : inclusionText.Trim()
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
                    ParameterName = "@StatusId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    Value = statusId == null ? DBNull.Value : statusId
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

            var submissionList = await dbContext.Set<SubmissionAggregationSP>().FromSqlRaw(@"
                EXECUTE [dbo].GetSubmissionAggregation @InclusionText, @ConferenceId, @TrackId, @StatusId, @SkipCount, @MaxResultCount
                ", sqlParameters.ToArray()).ToListAsync();

            return submissionList;
        }

        public async Task<SubmissionSummarySPO?> GetSubmissionSummaryAsync(Guid submissionId)
        {
            var dbContext = await GetDbContextAsync();

            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter() {
                    ParameterName = "@SubmissionId",
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Direction = ParameterDirection.Input,
                    Value = submissionId
                }
            };

            var resultList = await dbContext.Set<SubmissionSummarySPO>().FromSqlRaw(@"
                EXECUTE [dbo].GetSubmissionSummary @SubmissionId", sqlParameters.ToArray()).ToListAsync();

            if(resultList.Count > 0)
            {
                return resultList.First();
            }

            return null;
        }

        public override async Task<IQueryable<Submission>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(x => x.Conflicts)
                .Include(x => x.Clones)
                .ThenInclude(x => x.Reviews)
                .Include(x => x.CameraReadies);
        }
    }
}
