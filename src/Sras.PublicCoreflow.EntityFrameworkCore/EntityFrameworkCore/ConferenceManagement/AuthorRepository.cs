using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class AuthorRepository : EfCoreRepository<PublicCoreflowDbContext, Author, Guid>, IAuthorRepository
    {
        public AuthorRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> GetCountAuthorAggregation(Guid accountId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();

            // dung conference de lay danh sach submission cua conference do
            var submissionQueryable = (from t in dbContext.Set<Track>()
                                       join c in dbContext.Set<Conference>() on t.ConferenceId equals c.Id
                                       join s in dbContext.Set<Submission>() on t.Id equals s.TrackId
                                       where c.Id == conferenceId
                                       select s);


            // dung accountId lay partipantId, soi vo bang author
            var authorQueryable = (from p in dbContext.Set<Participant>()
                                   join a in dbContext.Set<IdentityUser>() on p.AccountId equals a.Id
                                   join auth in dbContext.Set<Author>() on p.Id equals auth.ParticipantId
                                   where a.Id == accountId
                                   select auth);

            // join 2 cai tren lay thong tin
            return (from s in submissionQueryable
                    join auth in authorQueryable on s.Id equals auth.SubmissionId
                    join t in dbContext.Set<Track>() on s.TrackId equals t.Id
                    join status in dbContext.Set<PaperStatus>() on s.NotifiedStatusId equals status.Id
                    select new AuthorSubmission
                    {
                        SubmissionId = s.Id,
                        SubmissionTitle = s.Title,
                        TrackId = t.Id,
                        TrackName = t.Name,
                        NotifiedStatusId = status.Id,
                        NotifiedStatusName = status.Name
                    }).Count();
        }

        public async Task<List<AuthorSubmission>> GetListAuthorAggregation(Guid accountId, Guid conferenceId, string sorting = AuthorConsts.DefaultSorting, int skipCount = 0, int maxResultCount = AuthorConsts.DefaultMaxResultCount)
        {
            var dbContext = await GetDbContextAsync();

            // dung conference de lay danh sach submission cua conference do
            var submissionQueryable = (from t in dbContext.Set<Track>()
                                  join c in dbContext.Set<Conference>() on t.ConferenceId equals c.Id
                                  join s in dbContext.Set<Submission>() on t.Id equals s.TrackId
                                  where c.Id == conferenceId
                                  select s);


            // dung accountId lay partipantId, soi vo bang author
            var authorQueryable = (from p in dbContext.Set<Participant>()
                                   join a in dbContext.Set<IdentityUser>() on p.AccountId equals a.Id
                                   join auth in dbContext.Set<Author>() on p.Id equals auth.ParticipantId
                                   where a.Id == accountId
                                   select auth);

            // join 2 cai tren lay thong tin
            return (from s in submissionQueryable
                                  join auth in authorQueryable on s.Id equals auth.SubmissionId
                                  join t in dbContext.Set<Track>() on s.TrackId equals t.Id
                                  join status in dbContext.Set<PaperStatus>() on s.NotifiedStatusId equals status.Id
                                  select new AuthorSubmission
                                  {
                                      SubmissionId = s.Id,
                                      SubmissionTitle = s.Title,
                                      TrackId = t.Id,
                                      TrackName = t.Name,
                                      NotifiedStatusId = status.Id,
                                      NotifiedStatusName = status.Name
                                  }).ToList();
        }
    }
}
