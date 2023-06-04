using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITrackAppService : IApplicationService
    {
        Task<TrackBriefInfo?> CreateAsync(Guid conferenceId, string trackName);
        Task<TrackBriefInfo?> UpdateAsync(Guid conferenceId, Guid trackId, string trackName);
        Task<List<TrackBriefInfo>?> GetAllAsync(Guid conferenceId);
    }
}
