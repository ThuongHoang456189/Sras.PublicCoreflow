using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

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
                    var track = new Track(trackId, isDefault, name, conferenceId, null, null, null, null, null, null);
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

    }
}
