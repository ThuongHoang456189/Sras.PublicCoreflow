using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using static Volo.Abp.Identity.IdentityPermissions;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class TrackRepository : EfCoreRepository<PublicCoreflowDbContext, Track, Guid>, ITrackRepository
    {
        private const string Reviewer = "Reviewer";
        private readonly IGuidGenerator _guidGenerator;
        public TrackRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetAllTrackByConferenceId(Guid conferenceId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var listTracks = await dbContext.Tracks.Where(t => t.ConferenceId == conferenceId).Select(tr => new
                {
                    Id = tr.Id,
                    Name = tr.Name,
                }).ToListAsync();
                var isSingleTrack = dbContext.Conferences.Where(c => c.Id == conferenceId).First().IsSingleTrack;
                return new
                {
                    tracks = listTracks,
                    isSingleTrack = isSingleTrack
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object> CreateTrackAsync(Guid conferenceId, string name)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if(dbContext.Conferences.Any(co => co.Id == conferenceId))
                {
                    var trackId = _guidGenerator.Create();
                    var isDefault = dbContext.Conferences.Where(c => c.Id == conferenceId).First().IsSingleTrack;
                    var track = new Track(trackId, isDefault, name, conferenceId, 
                                            null, null, null, null, null, 
                                            JsonSerializer.Serialize(TrackConsts.DefaultSubjectAreaRelevanceCoefficients));
                    dbContext.Conferences.Where(cc => cc.Id == conferenceId).First().Tracks.Add(track);
                    dbContext.Tracks.Add(track);
                    dbContext.SaveChanges();
                    return new
                    {
                        trackId = trackId,
                        trackName = name
                    };
                } else
                {
                    throw new Exception("ConferenceId not existing");
                }
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][CreateTrackAsync] " + ex.Message);
            }

        }

        public override async Task<IQueryable<Track>> WithDetailsAsync()
        {
            return (await GetQueryableAsync())
                .Include(x => x.SubjectAreas);
        }

        public async Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId, string roleName)
        {
            var dbContext = await GetDbContextAsync();
            if (roleName == null) roleName = "Track Chair";
            //1(roleName = "track chair")[ConferenceRole] => confRoleId

            if (!dbContext.ConferenceRoles.Any(cr => cr.Name == roleName)) throw new Exception("roleName : \"" + roleName + "\" is not exist");
            var conferenceRoleId = dbContext.ConferenceRoles.Where(c => c.Name == roleName).First().Id;
            //2(userId, conferenceId)[ConferenceAccount] => accConfId
            var accConfId = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId && ca.ConferenceId == conferenceId).First().Id;
            //3(confRoleId, accConfId)[Incumbent] => TrackIds
            var trackIds = dbContext.Incumbents.Where(i => i.ConferenceRoleId == conferenceRoleId && i.ConferenceAccountId == accConfId).Select(inc => inc.TrackId);
            //4 TrackIds.Select(trackId => {
            //    return {
            //    trackId: trackId,
            //		trackName: Tracks.Where(trackId).First(),
            //		roles: (trackId, accConfId)[Incumbent] => Roles.Select => id,name
            //    }
            //})
            //var DBTracks = await dbContext.Tracks.ToListAsync();
            //var DBIncumbents = await dbContext.Incumbents.ToListAsync();
            var tracks = trackIds.Select(tr => new
            {
                trackId = tr,
                trackName = dbContext.Tracks.Where(t => t.Id == tr).First().Name,
                roles = dbContext.Incumbents.Where(i => i.TrackId == tr && i.ConferenceAccountId == accConfId).OrderBy(incc => incc.ConferenceRole.Factor).Select(inc => new
                {
                    id = inc.ConferenceRole.Id,
                    name = inc.ConferenceRole.Name
                }).ToList()
            }).ToList();
            //5 Conference = Conferences.Where(conferenceId)
            var conference = dbContext.Conferences.Where(c => c.Id == conferenceId).First();
            if (conference == null) throw new Exception("ConferenceId Not Exist");
            //6 myConferences: (userId)[ConferenceAccount] => ConferenceIds => Conferences => [{ id, name}]
            var myConferences = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId).Select(caa => new
            {
                id = caa.Conference.Id,
                name = caa.Conference.FullName
            }).ToList();

            return new
            {
                tracks = tracks,
                conferenceName = conference.FullName,
                myConferences = myConferences,
                isSingleTrack = conference.IsSingleTrack
            };
        }

        public async Task<object> GetTracksAndRoleOfChair(Guid userId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            //1(roleName = "track chair")[ConferenceRole] => confRoleId

            var conferenceRoleId = dbContext.ConferenceRoles.Where(c => c.Name == "Chair").First().Id;
            //2(userId, conferenceId)[ConferenceAccount] => accConfId
            var accConfId = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId && ca.ConferenceId == conferenceId).First().Id;
            //3(confRoleId, accConfId)[Incumbent] => TrackIds
            var trackIds = dbContext.Tracks.Where(t => t.ConferenceId == conferenceId).Select(inc => inc.Id);
            //4 TrackIds.Select(trackId => {
            //    return {
            //    trackId: trackId,
            //		trackName: Tracks.Where(trackId).First(),
            //		roles: (trackId, accConfId)[Incumbent] => Roles.Select => id,name
            //    }
            //})
            //var DBTracks = await dbContext.Tracks.ToListAsync();
            //var DBIncumbents = await dbContext.Incumbents.ToListAsync();
            var tracks = trackIds.Select(tr =>
             new
                {
                    trackId = tr,
                    trackName = dbContext.Tracks.Where(t => t.Id == tr).First().Name,
                    roles = new List<object> { new { id = conferenceRoleId, name = "Chair"}, dbContext.Incumbents
                                                                                                    .Where(i => i.TrackId == tr && i.ConferenceAccountId == accConfId)
                                                                                                    .OrderBy(incc => incc.ConferenceRole.Factor)
                                                                                                    .Select(inc => new
                                                                                                            {
                                                                                                                id = inc.ConferenceRole.Id,
                                                                                                                name = inc.ConferenceRole.Name
                                                                                                            }).ToList() }
                }).ToList();
            //5 Conference = Conferences.Where(conferenceId)
            var conference = dbContext.Conferences.Where(c => c.Id == conferenceId).First();
            if (conference == null) throw new Exception("ConferenceId Not Exist");
            //6 myConferences: (userId)[ConferenceAccount] => ConferenceIds => Conferences => [{ id, name}]
            var myConferences = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId).Select(caa => new
            {
                id = caa.Conference.Id,
                name = caa.Conference.FullName
            }).ToList();

            return new
            {
                tracks = tracks,
                conferenceName = conference.FullName,
                myConferences = myConferences,
                isSingleTrack = conference.IsSingleTrack
            };
        }

    }
}
