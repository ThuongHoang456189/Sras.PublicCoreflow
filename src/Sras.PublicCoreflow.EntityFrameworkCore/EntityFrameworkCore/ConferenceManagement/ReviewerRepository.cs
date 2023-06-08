using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ReviewerRepository : EfCoreRepository<PublicCoreflowDbContext, Reviewer, Guid>, IReviewerRepository
    {
        private const string Reviewer = "Reviewer";

        public ReviewerRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
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
    }
}
