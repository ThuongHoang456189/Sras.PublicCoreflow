using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Data;
using System.Linq.Dynamic.Core;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp.Domain.Repositories;
using Volo.Abp;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class IncumbentRepository : EfCoreRepository<PublicCoreflowDbContext, Incumbent, Guid>, IIncumbentRepository
    {
        private const string Chair = "Chair";
        private const string Author = "Author";

        public IncumbentRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<bool> IsConferenceChair(Guid accountId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();

            var query = (from i in dbContext.Set<Incumbent>()
                         join ca in (from ca1 in dbContext.Set<ConferenceAccount>()
                                     join c in (from c1 in dbContext.Set<Conference>() select c1).Where(x => x.Id == conferenceId) on ca1.ConferenceId equals c.Id
                                     join a in (from a1 in dbContext.Set<IdentityUser>() select a1).Where(x => x.Id == accountId) on ca1.AccountId equals a.Id
                                     select ca1)
                         on i.ConferenceAccountId equals ca.Id
                         join r in dbContext.Set<ConferenceRole>() on i.ConferenceRoleId equals r.Id
                         select r)
                        .Where(x => x.Name.Equals("Chair"));

            var list = await query.ToListAsync();
            return list.Any();
        }

        public async Task<List<RoleTrackOperation>> GetRoleTrackOperationTableAsync(Guid accountId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();

            var query = from ca in ((from ca1 in dbContext.Set<ConferenceAccount>()
                                     select ca1).Where(x => x.AccountId == accountId))
                        join conf in ((from conf1 in dbContext.Set<Conference>()
                                       select conf1).Where(x => x.Id == conferenceId)) on ca.ConferenceId equals conf.Id
                        join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                        select new RoleTrackOperation(
                                ca.Id,
                                i.Id,
                                i.ConferenceRoleId,
                                i.TrackId,
                                RoleTrackManipulationOperators.None
                            );

            return await query.ToListAsync();
        }

        private class IncumbentRow
        {
            internal Guid IncumbentId { get; set; }
            internal Guid RoleId { get; set; }
            internal string RoleName { get; set; }
            public int Factor { get; set; }
            internal Guid? TrackId { get; set; }
            internal string? TrackName { get; set; }
        }

        public async Task<ConferenceParticipationInfo?> GetConferenceParticipationInfoAsync(Guid accountId, Guid conferenceId, Guid? trackId)
        {
            var dbContext = await GetDbContextAsync();

            ConferenceParticipationInfo participation = new ConferenceParticipationInfo();

            var userQueryable = (from u in dbContext.Set<IdentityUser>()
                                 select u).Where(x => x.Id == accountId);
            var user = userQueryable.SingleOrDefault();

            if (user == null)
                return null;

            var participantQueryable = (from p in dbContext.Set<Participant>()
                                        select p).Where(y => y.AccountId == accountId);

            var participant = participantQueryable.SingleOrDefault();

            participation.Id = user.Id;
            participation.Email = user.Email;
            participation.FirstName = user.Name;
            participation.MiddleName = user.GetProperty<string?>(nameof(participation.MiddleName));
            participation.LastName = user.Surname;
            participation.Organization = user.GetProperty<string?>(nameof(participation.Organization));
            participation.ParticipantId = participant == null ? null : participant.Id;

            var incumbentQueryable = from ca in dbContext.Set<ConferenceAccount>()
                                     join u in ((from u1 in dbContext.Set<IdentityUser>() select u1).Where(x => x.Id == accountId)) on ca.AccountId equals u.Id
                                     join c in ((from c1 in dbContext.Set<Conference>() select c1).Where(x => x.Id == conferenceId)) on ca.ConferenceId equals c.Id
                                     join i in dbContext.Set<Incumbent>() on ca.Id equals i.ConferenceAccountId
                                     select i;

            var incumbentRowQueryable = (from i in incumbentQueryable
                                         join r in dbContext.Set<ConferenceRole>() on i.ConferenceRoleId equals r.Id
                                         join t in dbContext.Set<Track>() on i.TrackId equals t.Id into gj
                                         from subtrack in gj.DefaultIfEmpty()
                                         select new IncumbentRow
                                         {
                                             IncumbentId = i.Id,
                                             RoleId = r.Id,
                                             RoleName = r.Name,
                                             Factor = r.Factor,
                                             TrackId = subtrack == null ? null : subtrack.Id,
                                             TrackName = subtrack == null ? null : subtrack.Name
                                         })
                                        .WhereIf(trackId != null, x => x.TrackId == trackId)
                                        .OrderBy(ConferenceRoleConsts.DefaultSorting);

            var incumbentRowList = await incumbentRowQueryable.ToListAsync();

            if (incumbentRowList != null && incumbentRowList.Count > 0)
            {
                participation.Roles = new List<RoleWithEngagedTrackInfo>();

                int currentRoleFactor = -1;
                RoleWithEngagedTrackInfo? currentRole = null;

                for (int i = 0; i < incumbentRowList.Count; i++)
                {
                    if (currentRoleFactor != incumbentRowList[i].Factor)
                    {
                        if (currentRole != null)
                        {
                            participation.Roles.Add(currentRole);
                        }

                        currentRoleFactor = incumbentRowList[i].Factor;
                        currentRole = new RoleWithEngagedTrackInfo(incumbentRowList[i].RoleId, incumbentRowList[i].RoleName, incumbentRowList[i].Factor);
                    }

                    if (currentRole != null && incumbentRowList[i].TrackId != null && incumbentRowList[i].TrackName != null)
                    {
                        if (currentRole.EngagedTracks == null)
                        {
                            currentRole.EngagedTracks = new List<TrackBriefInfo>();
                        }
                        currentRole.EngagedTracks.Add(new TrackBriefInfo(incumbentRowList[i].TrackId.GetValueOrDefault(), incumbentRowList[i].TrackName ?? ""));
                    }

                    // Closing
                    if (i == incumbentRowList.Count - 1 && currentRole != null)
                    {
                        participation.Roles.Add(currentRole);
                    }
                }
            }

            return participation;
        }

        public class ConferenceAccountRow
        {
            public Guid ConferenceAccountId { get; set; }
            public Guid AccountId { get; set; }
            public string Email { get; set; }
        }

        public class RoleRow
        {
            public Guid ConferenceAccountId { get; set; }
            public Guid RoleId { get; set; }
            public Guid? TrackId { get; set; }
        }

        public class ConferenceAccountRowComparer : IEqualityComparer<ConferenceAccountRow>
        {
            public bool Equals(ConferenceAccountRow? x, ConferenceAccountRow? y)
            {
                if (Object.ReferenceEquals(x, y)) return true;

                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.ConferenceAccountId == y.ConferenceAccountId 
                    && x.AccountId == y.AccountId 
                    && x.Email.ToLower().Equals(y.Email.ToLower());
            }

            public int GetHashCode([DisallowNull] ConferenceAccountRow obj)
            {
                if (Object.ReferenceEquals(obj, null)) return 0;

                int hashConferenceAccountId = obj.ConferenceAccountId.GetHashCode();
                int hashAccountId = obj.AccountId.GetHashCode();
                int hashEmail = obj.Email.GetHashCode();

                return hashConferenceAccountId ^ hashAccountId ^ hashEmail;
            }
        }

        public class RoleRowComparer : IEqualityComparer<RoleRow>
        {
            public bool Equals(RoleRow? x, RoleRow? y)
            {
                if (Object.ReferenceEquals(x, y)) return true;

                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.RoleId == y.RoleId;
            }

            public int GetHashCode([DisallowNull] RoleRow obj)
            {
                if (Object.ReferenceEquals(obj, null)) return 0;

                int hashRoleId = obj.RoleId.GetHashCode();
                int hashConferenceAccountId = obj.ConferenceAccountId.GetHashCode();
                int hashTrackId = obj.TrackId == null ? 0 : obj.TrackId.GetHashCode();

                return hashConferenceAccountId ^ hashRoleId ^ hashTrackId;
            }
        }

        public async Task<int> GetConferenceUserCountAsync(Guid conferenceId, Guid? trackId)
        {
            var dbContext = await GetDbContextAsync();

            var roleQueryable = from r in dbContext.Set<ConferenceRole>() select r;
            var roleList = await roleQueryable.ToListAsync();
            var roleMap = new Dictionary<string, Guid>();
            var roleNameMap = new Dictionary<Guid, string>();
            roleList.ForEach(x =>
            {
                roleMap.Add(x.Name, x.Id);
                roleNameMap.Add(x.Id, x.Name);
            });

            var conferenceAccountRowQueryable = (from ca in dbContext.Set<ConferenceAccount>()
                                                 join u in ((from u1 in dbContext.Set<IdentityUser>() select u1)) on ca.AccountId equals u.Id
                                                 join c in ((from c1 in dbContext.Set<Conference>() select c1).Where(x => x.Id == conferenceId)) on ca.ConferenceId equals c.Id
                                                 join i in ((from i1 in dbContext.Set<Incumbent>() select i1).WhereIf(trackId != null, x => x.TrackId == trackId || x.ConferenceRoleId == roleMap.GetValueOrDefault(Chair))) on ca.Id equals i.ConferenceAccountId
                                                 select new ConferenceAccountRow
                                                 {
                                                     ConferenceAccountId = ca.Id,
                                                     AccountId = u.Id,
                                                     Email = u.Email,
                                                 })
                                          .OrderBy(AccountConsts.DefaultSorting);

            var conferenceAccountRowList = await conferenceAccountRowQueryable.ToListAsync();

            for (int i = 0; i < conferenceAccountRowList.Count; i++)
            {
                for (int j = i + 1; j < conferenceAccountRowList.Count; j++)
                {
                    if (conferenceAccountRowList[i].ConferenceAccountId == conferenceAccountRowList[j].ConferenceAccountId
                        && conferenceAccountRowList[i].AccountId == conferenceAccountRowList[j].AccountId
                        && conferenceAccountRowList[i].Email.ToLower().Equals(conferenceAccountRowList[j].Email.ToLower()))
                    {
                        conferenceAccountRowList.Remove(conferenceAccountRowList[j]);
                        j--;
                    }
                }
            }

            return conferenceAccountRowList.Count;
        }

        public async Task<List<ConferenceParticipationBriefInfo>> GetConferenceUserListAsync(Guid conferenceId, Guid? trackId, int skipCount = 0, int maxResultCount = ConferenceConsts.MaxConferenceUserEachPage)
        {
            if (maxResultCount > ConferenceConsts.MaxConferenceUserEachPage)
                maxResultCount = ConferenceConsts.MaxConferenceUserEachPage;

            var dbContext = await GetDbContextAsync();

            var roleQueryable = from r in dbContext.Set<ConferenceRole>() select r;
            var roleList = await roleQueryable.ToListAsync();
            var roleMap = new Dictionary<string, Guid>();
            var roleNameMap = new Dictionary<Guid, string>();
            roleList.ForEach(x =>
            {
                roleMap.Add(x.Name, x.Id);
                roleNameMap.Add(x.Id, x.Name);
            });

            List<ConferenceParticipationBriefInfo> result = new List<ConferenceParticipationBriefInfo>();

            try
            {
                // ConferenceAccount thoa dieu kien
                // Neu trackId null thi lay tat ca
                // Neu trackId co thi lay role chair neu co, va nhung role gan chat voi trackId do

                var conferenceAccountRowQueryable = (from ca in dbContext.Set<ConferenceAccount>()
                                          join u in ((from u1 in dbContext.Set<IdentityUser>() select u1)) on ca.AccountId equals u.Id
                                          join c in ((from c1 in dbContext.Set<Conference>() select c1).Where(x => x.Id == conferenceId)) on ca.ConferenceId equals c.Id
                                          join i in ((from i1 in dbContext.Set<Incumbent>() select i1).WhereIf(trackId != null, x => x.TrackId == trackId || x.ConferenceRoleId == roleMap.GetValueOrDefault(Chair))) on ca.Id equals i.ConferenceAccountId
                                          select new ConferenceAccountRow
                                          {
                                              ConferenceAccountId = ca.Id,
                                              AccountId = u.Id,
                                              Email = u.Email,
                                          })
                                          .OrderBy(AccountConsts.DefaultSorting);

                var conferenceAccountRowList = await conferenceAccountRowQueryable.ToListAsync();

                // Clean duplicate in incumbentList (not paged)
                for(int i =0; i < conferenceAccountRowList.Count; i++)
                {
                    for(int j = i + 1; j < conferenceAccountRowList.Count; j++)
                    {
                        if (conferenceAccountRowList[i].ConferenceAccountId == conferenceAccountRowList[j].ConferenceAccountId
                            && conferenceAccountRowList[i].AccountId == conferenceAccountRowList[j].AccountId
                            && conferenceAccountRowList[i].Email.ToLower().Equals(conferenceAccountRowList[j].Email.ToLower()))
                        {
                            conferenceAccountRowList.Remove(conferenceAccountRowList[j]);
                            j--;
                        }
                    }
                }

                // Aggregate role for each conference account + paged list
                int startIndex = skipCount;
                int added = 0;

                for(int i = startIndex; i < conferenceAccountRowList.Count; i++)
                {
                    ConferenceParticipationBriefInfo participation = new ConferenceParticipationBriefInfo();

                    var userQueryable = (from u in dbContext.Set<IdentityUser>()
                                         select u).Where(y => y.Id == conferenceAccountRowList[i].AccountId);
                    var user = userQueryable.SingleOrDefault();

                    var participantQueryable = (from p in dbContext.Set<Participant>()
                                                select p).Where(y => y.AccountId == conferenceAccountRowList[i].AccountId);

                    var participant = participantQueryable.SingleOrDefault();

                    if (user != null)
                    {
                        participation.Id = user.Id;
                        participation.Email = user.Email;
                        participation.FirstName = user.Name;
                        participation.MiddleName = user.GetProperty<string?>(nameof(participation.MiddleName));
                        participation.LastName = user.Surname;
                        participation.Organization = user.GetProperty<string?>(nameof(participation.Organization));
                        participation.ParticipantId = participant == null ? null : participant.Id;

                        var incumbentQueryable = (from incumbent in dbContext.Set<Incumbent>()
                                                  select new RoleRow
                                                  {
                                                      ConferenceAccountId = incumbent.ConferenceAccountId,
                                                      RoleId = incumbent.ConferenceRoleId,
                                                      TrackId = incumbent.TrackId
                                                  })
                                                  .Where(y => y.ConferenceAccountId == conferenceAccountRowList[i].ConferenceAccountId)
                                                  .WhereIf(trackId != null, y => y.TrackId == trackId || y.RoleId == roleMap.GetValueOrDefault(Chair));
                        var incumbentList = await incumbentQueryable.ToListAsync();

                        // Clean incumbentList
                        for(int p = 0; p < incumbentList.Count; p++)
                        {
                            for(int q = p + 1;  q < incumbentList.Count; q++)
                            {
                                if (incumbentList[p].RoleId == incumbentList[q].RoleId)
                                {
                                    incumbentList.Remove(incumbentList[q]);
                                    q--;
                                }
                            }
                        }

                        if (incumbentList != null && incumbentList.Any())
                        {
                            incumbentList.ForEach(y =>
                            {
                                participation.Roles.Add(roleNameMap.GetValueOrDefault(y.RoleId) ?? string.Empty);
                            });
                        }
                    }

                    result.Add(participation);

                    added++;
                    if (added >= maxResultCount)
                        break;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return result;
        }

        public async Task<List<AuthorOperation>> GetAuthorOperationTableAsync(Guid conferenceId, Guid trackId, List<AuthorInput> authors)
        {
            var dbContext = await GetDbContextAsync();
            
            var authorRole = (from r in dbContext.Set<ConferenceRole>() select r)
                                .Where(x => x.Name.Equals(Author))
                                .FirstOrDefault();
            if(authorRole == null) 
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceRoleAuthorNotFound);
            }

            var result = new List<AuthorOperation>();

            authors.ForEach(auth =>
            {
                AuthorOperation operation = new AuthorOperation();
                operation.ParticipantId = auth.ParticipantId;

                var participant = (from p in dbContext.Set<Participant>() select p)
                                            .Where(x => x.Id == auth.ParticipantId && x.AccountId != null)
                                            .FirstOrDefault();

                operation.AccountId = participant == null ? null : participant.AccountId;

                if(participant != null)
                {
                    operation.IsPrimaryContact = auth.IsPrimaryContact;

                    var conferenceAccount = (from ca in dbContext.Set<ConferenceAccount>() select ca)
                                                    .Where(x => x.ConferenceId == conferenceId && x.AccountId == participant.AccountId)
                                                    .FirstOrDefault();

                    if(conferenceAccount != null)
                    {
                        operation.ConferenceAccountId = conferenceAccount.Id;

                        var incumbent = (from i in dbContext.Set<Incumbent>()
                                         select i)
                                         .Where(x => x.ConferenceRoleId == authorRole.Id && x.TrackId == trackId && x.ConferenceAccountId == conferenceAccount.Id)
                                         .FirstOrDefault();

                        if(incumbent != null)
                        {
                            operation.AuthorRoleId = authorRole.Id;
                            operation.Operation = AuthorManipulationOperators.None;
                        }
                        else
                        {
                            operation.AuthorRoleId = authorRole.Id;
                            operation.Operation = AuthorManipulationOperators.UpAdd;
                        }
                    }
                    else
                    {
                        operation.ConferenceAccountId = null;
                        operation.AuthorRoleId = authorRole.Id;
                        operation.Operation = AuthorManipulationOperators.Add2;
                    }
                }
                else
                {
                    if (auth.IsPrimaryContact)
                    {
                        throw new BusinessException(PublicCoreflowDomainErrorCodes.PrimaryContactCannotSetToNonAccountParticipant);
                    }
                    operation.IsPrimaryContact = auth.IsPrimaryContact;
                    operation.AccountId = null;
                    operation.ConferenceAccountId = null;
                    operation.AuthorRoleId = null;
                    operation.Operation = AuthorManipulationOperators.None;
                }

                result.Add(operation);
            });

            return result;
        }

        public async Task<List<ConferenceAccount>> GetAllAccountsWithProfileListAsync()
        {
            var dbContext = await GetDbContextAsync();
            var result = await dbContext.ConferenceAccounts.ToListAsync();
            
            return result;
        }
    }
}
