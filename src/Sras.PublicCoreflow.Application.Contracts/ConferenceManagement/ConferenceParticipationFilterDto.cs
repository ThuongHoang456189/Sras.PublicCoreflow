using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceParticipationFilterDto 
    {
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
        public int SkipCount { get; set; }
        public int MaxResultCount { get; set; }
    }
}
