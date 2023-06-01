using Sras.PublicCoreflow.ConferenceManagement;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using System.Linq.Dynamic.Core;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ConferenceRepository : EfCoreRepository<PublicCoreflowDbContext, Conference, Guid>, IConferenceRepository
    {
        public ConferenceRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        private bool IsInclusivelyMatched(ConferenceWithBriefInfo conference, string inclusionText)
        {
            inclusionText = inclusionText.Trim().ToLower();
            return conference.FullName.ToLower().Contains(inclusionText)
                || conference.ShortName.ToLower().Contains(inclusionText)
                || conference.City.ToLower().Contains(inclusionText)
                || conference.Country.ToLower().Contains(inclusionText);
        }

        private bool IsAfterDate(DateTime? milestone, DateTime? date)
        {
            if (milestone == null || date == null)
                return false;
            return date.Value.Date >= milestone.Value.Date;
        }

        private bool IsBeforeDate(DateTime? milestone, DateTime? date)
        {
            if (milestone == null || date == null)
                return false;
            return date.Value.Date <= milestone.Value.Date;
        }

        public async Task<int> GetCountAsync(string? inclusionText = null, string? fullName = null, string? shortName = null, string? city = null, string? country = null, DateTime? startDate = null, DateTime? endDate = null, Guid? accountId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            IQueryable<ConferenceWithBriefInfo> conferenceQuery = (from @conference in dbContext.Set<Conference>()
                                                                   select new ConferenceWithBriefInfo
                                                                   {
                                                                       Id = @conference.Id,
                                                                       FullName = @conference.FullName,
                                                                       ShortName = @conference.ShortName,
                                                                       City = @conference.City,
                                                                       Country = @conference.Country,
                                                                       StartDate = @conference.StartDate,
                                                                       EndDate = @conference.EndDate,
                                                                       WebsiteLink = @conference.WebsiteLink,
                                                                       Logo = @conference.Logo,
                                                                       IsSingleTrack = @conference.IsSingleTrack
                                                                   })
                                                                   .WhereIf(!string.IsNullOrWhiteSpace(inclusionText), x => IsInclusivelyMatched(x, inclusionText))
                                                                   .WhereIf(!string.IsNullOrWhiteSpace(fullName), x => x.FullName.ToLower().Contains(fullName.Trim().ToLower()))
                                                                   .WhereIf(!string.IsNullOrWhiteSpace(shortName), x => x.ShortName.ToLower().Contains(shortName.Trim().ToLower()))
                                                                   .WhereIf(!string.IsNullOrWhiteSpace(city), x => x.City.ToLower().Contains(city.Trim().ToLower()))
                                                                   .WhereIf(!string.IsNullOrWhiteSpace(country), x => x.Country.ToLower().Contains(country.Trim().ToLower()))
                                                                   .WhereIf(startDate != null, x => IsAfterDate(x.StartDate, startDate))
                                                                   .WhereIf(endDate != null, x => IsBeforeDate(x.EndDate, endDate));

            var query = accountId == null ? conferenceQuery :
                         (from ca in dbContext.Set<ConferenceAccount>()
                          join @conference in conferenceQuery on ca.ConferenceId equals @conference.Id
                          join acc in dbContext.Set<IdentityUser>() on ca.AccountId equals acc.Id
                          where ca.AccountId == accountId
                          select @conference);

            return await query.CountAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<List<IncumbentOperation>> GetIncumbentOperationTableAsync(Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();

            var query = from ca in ((from ca1 in dbContext.Set<ConferenceAccount>()
                                     select ca1).Where(x => x.ConferenceId == conferenceId))
                        join acc in dbContext.Set<IdentityUser>() on ca.AccountId equals acc.Id
                        join inc in dbContext.Set<Incumbent>() on ca.Id equals inc.ConferenceAccountId
                        select new IncumbentOperation(ca.Id,
                                                      ca.AccountId,
                                                      inc.Id,
                                                      inc.ConferenceRoleId,
                                                      IncumbentManipulationOperators.None,
                                                      inc.IsPrimaryContact);

            return await query.ToListAsync();
        }

        public async Task<List<ConferenceWithBriefInfo>> GetListAsync(string? sorting = null, int skipCount = 0, int maxResultCount = int.MaxValue, string? inclusionText = null, string? fullName = null, string? shortName = null, string? city = null, string? country = null, DateTime? startDate = null, DateTime? endDate = null, Guid? accountId = null, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            IQueryable<ConferenceWithBriefInfo> conferenceQuery =
            (from @conference in dbContext.Set<Conference>()
             select new ConferenceWithBriefInfo
             {
                 Id = @conference.Id,
                 FullName = @conference.FullName,
                 ShortName = @conference.ShortName,
                 City = @conference.City,
                 Country = @conference.Country,
                 StartDate = @conference.StartDate,
                 EndDate = @conference.EndDate,
                 WebsiteLink = @conference.WebsiteLink,
                 Logo = @conference.Logo,
                 IsSingleTrack = @conference.IsSingleTrack
             })
             .WhereIf(!string.IsNullOrWhiteSpace(inclusionText), x => IsInclusivelyMatched(x, inclusionText))
             .WhereIf(!string.IsNullOrWhiteSpace(fullName), x => x.FullName.ToLower().Contains(fullName.Trim().ToLower()))
             .WhereIf(!string.IsNullOrWhiteSpace(shortName), x => x.ShortName.ToLower().Contains(shortName.Trim().ToLower()))
             .WhereIf(!string.IsNullOrWhiteSpace(city), x => x.City.ToLower().Contains(city.Trim().ToLower()))
             .WhereIf(!string.IsNullOrWhiteSpace(country), x => x.Country.ToLower().Contains(country.Trim().ToLower()))
             .WhereIf(startDate != null, x => IsAfterDate(x.StartDate, startDate))
             .WhereIf(endDate != null, x => IsBeforeDate(x.EndDate, endDate))
             .OrderBy(string.IsNullOrWhiteSpace(sorting) ? ConferenceConsts.DefaultSorting : sorting)
             .PageBy(skipCount, maxResultCount);

            var query = accountId == null ? conferenceQuery :
                         (from ca in dbContext.Set<ConferenceAccount>()
                          join @conference in conferenceQuery on ca.ConferenceId equals @conference.Id
                          join acc in dbContext.Set<IdentityUser>() on ca.AccountId equals acc.Id
                          where ca.AccountId == accountId
                          select @conference)
                         .OrderBy(string.IsNullOrWhiteSpace(sorting) ? ConferenceConsts.DefaultSorting : sorting)
                         .PageBy(skipCount, maxResultCount);

            return await query.ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<bool> IsExistAsync(string fullName, string shortName, string city, string country, DateTime startDate, DateTime endDate)
        {
            var dbSet = await GetDbSetAsync();
            return await dbSet.AnyAsync(x =>
            string.Equals(x.FullName.ToLower(), fullName.Trim().ToLower())
            && string.Equals(x.ShortName.ToLower(), shortName.Trim().ToLower())
            && string.Equals(x.City.ToLower(), city.Trim().ToLower())
            && string.Equals(x.Country.ToLower(), country.Trim().ToLower())
            && ((startDate.Date >= x.StartDate.Date && startDate.Date <= x.EndDate)
            || (endDate.Date >= x.StartDate && endDate.Date <= x.EndDate)));
        }

        public override async Task<IQueryable<Conference>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(x => x.ConferenceAccounts)
                .ThenInclude(x => x.Incumbents);
        }
    }
}
