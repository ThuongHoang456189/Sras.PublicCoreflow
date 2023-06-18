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

        //public async Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId, string roleName)
        //{
        //    var dbContext = await GetDbContextAsync();
        //    if (roleName == null) roleName = "Track Chair";
        //    //1(roleName = "track chair")[ConferenceRole] => confRoleId

        //    if (!dbContext.ConferenceRoles.Any(cr => cr.Name == roleName)) throw new Exception("roleName : \"" + roleName + "\" is not exist");
        //    var conferenceRoleId = dbContext.ConferenceRoles.Where(c => c.Name == roleName).First().Id;
        //    //2(userId, conferenceId)[ConferenceAccount] => accConfId
        //    var accConfId = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId && ca.ConferenceId == conferenceId).First().Id;
        //    //3(confRoleId, accConfId)[Incumbent] => TrackIds
        //    var trackIds = dbContext.Incumbents.Where(i => i.ConferenceRoleId == conferenceRoleId && i.ConferenceAccountId == accConfId).Select(inc => inc.TrackId);
        //    //4 TrackIds.Select(trackId => {
        //    //    return {
        //    //    trackId: trackId,
        //    //		trackName: Tracks.Where(trackId).First(),
        //    //		roles: (trackId, accConfId)[Incumbent] => Roles.Select => id,name
        //    //    }
        //    //})
        //    //var DBTracks = await dbContext.Tracks.ToListAsync();
        //    //var DBIncumbents = await dbContext.Incumbents.ToListAsync();
        //    var tracks = trackIds.Select(tr => new
        //    {
        //        trackId = tr,
        //        trackName = dbContext.Tracks.Where(t => t.Id == tr).First().Name,
        //        roles = dbContext.Incumbents.Where(i => i.TrackId == tr && i.ConferenceAccountId == accConfId).OrderBy(incc => incc.ConferenceRole.Factor).Select(inc => new
        //        {
        //            id = inc.ConferenceRole.Id,
        //            name = inc.ConferenceRole.Name
        //        }).ToList()
        //    }).ToList();
        //    //5 Conference = Conferences.Where(conferenceId)
        //    var conference = dbContext.Conferences.Where(c => c.Id == conferenceId).First();
        //    if (conference == null) throw new Exception("ConferenceId Not Exist");
        //    //6 myConferences: (userId)[ConferenceAccount] => ConferenceIds => Conferences => [{ id, name}]
        //    var myConferences = dbContext.ConferenceAccounts.Where(ca => ca.AccountId == userId).Select(caa => new
        //    {
        //        id = caa.Conference.Id,
        //        name = caa.Conference.FullName
        //    }).ToList();

        //    return new
        //    {
        //        tracks = tracks,
        //        conferenceName = conference.FullName,
        //        myConferences = myConferences,
        //        isSingleTrack = conference.IsSingleTrack
        //    };
        //}
        public async Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId) 
        {
            if (isUserJoinConference(userId, conferenceId).Result)
            {
                return GetTrackOfTrackChairAndSubRoleEachTrack(userId, conferenceId).Result;
            } else
            {
                var dbContext = await GetDbContextAsync();
                var Author = dbContext.ConferenceRoles.Where(cr => cr.Name == "Author").First();
                var id = Author.Id;
                var name = Author.Name;
                return new
                {
                    myConferences = GetMyConference(userId),
                    tracks = new List<object> { },
                    roles = new List<object> {
                        new {
                            trackId = (string) null,
                            subRoles = new List<object> { 
                                new {
                                    id,
                                    name 
                                }
                            },
                        }, 
                    },
                };
            }
        }

        public async Task<object> GetTrackOfTrackChairAndSubRoleEachTrack(Guid userId, Guid conferenceId)
        {
            // get confAccId
            var dbContext = await GetDbContextAsync();
            var conferenceRoles = await dbContext.ConferenceRoles.ToListAsync();
            var chair = conferenceRoles.Where(cr => cr.Name == "Chair").First();
            var author = conferenceRoles.Where(cr => cr.Name == "Author").First();
            var reviewer = conferenceRoles.Where(cr => cr.Name == "Reviewer").First();
            var trackChair = conferenceRoles.Where(cr => cr.Name == "Track Chair").First();
            var confAccId = dbContext.ConferenceAccounts.Where(c => c.AccountId == userId && c.ConferenceId == conferenceId).First().Id;
            var defaultSubRoles = new List<object> { };
            defaultSubRoles.Add(new // default author
            {
                id = author.Id,
                name = author.Name,
            });
            if (haveReviewerRole(confAccId, conferenceRoles).Result)
            {
                defaultSubRoles.Add(new // default reviewer
                {
                    id = reviewer.Id,
                    name = reviewer.Name,
                });
            }
            if (isChairInConference(confAccId).Result)
            {
                defaultSubRoles.Add(new // if user is chair always have chair role in subroles
                {
                    id = chair.Id,
                    name = chair.Name,
                });
            }

            var tracks = dbContext.Incumbents.Include(i => i.Track).Include(ii => ii.ConferenceRole)
                .Where(i => i.ConferenceAccountId == confAccId)
                .Where(ii => ii.TrackId != null)
                .GroupBy(i => i.TrackId);
            
            var roles = new List<object>(){ };
            var subRoles = new List<object>() { };
            var listTrackIdInTracks = new List<Guid>() { };
            foreach (var group in tracks)
            {

                if (group.Key != null)
                {
                    subRoles = defaultSubRoles;
                    if (group.Where(g => g.ConferenceRole.Id == author.Id).Count() > 0) subRoles.RemoveAt(0);
                    if (group.Where(g => g.ConferenceRole.Id == reviewer.Id).Count() > 0) subRoles.RemoveAt(0);
                    subRoles.AddRange(group.OrderBy(g => g.ConferenceRole.Factor).Select(inc => new
                    {
                        id = inc.ConferenceRole.Id,
                        name = inc.ConferenceRole.Name,
                    }).ToList());
                    
                    if (group.Any(g => g.ConferenceRoleId == trackChair.Id)) {
                        roles.Add(new
                        {
                            trackId = group.Key,
                            subRoles = subRoles
                        });
                    } else
                    {
                        roles.Add(new
                        {
                            trackId = (string)null,
                            subRoles = subRoles
                        });
                    }
                }
            }

            return new {
                myConferences = GetMyConference(userId).Result,
                tracks = getTrackHaveTrackChairRole(confAccId).Result,
                roles,
                isSingleTrack = isSingleTrack(conferenceId).Result
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


        private async Task<bool> isUserJoinConference(Guid userId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.ConferenceAccounts.Include(ca => ca.Incumbents)
                .Where(ca => ca.AccountId == userId && ca.ConferenceId == conferenceId)
                .First().Incumbents.Count() > 0;

        }

        private async Task<IEnumerable<object>> GetMyConference(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.ConferenceAccounts.Include(ca => ca.Conference).Where(ca => ca.AccountId == userId)
                .Select(ca => new
                {
                    id = ca.ConferenceId,
                    name = ca.Conference.FullName
                }).ToList();
        }

        private async Task<IEnumerable<object>> getTrackHaveTrackChairRole(Guid conAccId)
        {
            var dbContext = await GetDbContextAsync();
            var trackChairRoleId = dbContext.ConferenceRoles.Where(cr => cr.Name == "Track Chair").First().Id;
            return dbContext.Incumbents.Include(i => i.Track).Where(i => i.ConferenceAccountId == conAccId && i.ConferenceRoleId == trackChairRoleId)
                .Select(ii => new
                {
                    id = ii.Track.Id,
                    name = ii.Track.Name
                }).ToList();
        }

        private async Task<bool> isSingleTrack(Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Conferences.Where(c => c.Id == conferenceId).First().IsSingleTrack;
        }

        private async Task<bool> isChairInConference(Guid conAccId) 
        {
            var dbContext = await GetDbContextAsync();
            var chairId = dbContext.ConferenceRoles.Where(cr => cr.Name == "Chair").First().Id;
            return dbContext.Incumbents.Where(i => i.ConferenceRoleId == chairId && i.ConferenceAccountId == conAccId).Count() > 0;
        }

        private async Task<bool> haveReviewerRole(Guid confAccId, List<ConferenceRole> conferenceRoles)
        {
            var dbContext = await GetDbContextAsync();
            var reviwerId = conferenceRoles.Where(cr => cr.Name == "Reviewer").First().Id;
            return dbContext.Incumbents.Where(i => i.ConferenceAccountId == confAccId)
                .Any(i => i.ConferenceRoleId == reviwerId);
        }

    }
}
