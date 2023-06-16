using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface ITrackRepository
    {
        Task<object> CreateTrackAsync(Guid conferenceId, string name);
        Task<object> GetAllTrackByConferenceId(Guid conferenceId);
        Task<object> GetTrackOfTrackChairAndSubRoleEachTrack(Guid userId, Guid conferenceId);
        Task<object> GetTracksAndRoleOfChair(Guid userId, Guid conferenceId);
        Task<object> GetTracksAndRoleOfUser(Guid userId, Guid conferenceId);
    }
}
