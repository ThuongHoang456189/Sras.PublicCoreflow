using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAggregationListFilterDto : PagedAndSortedResultRequestDto
    {
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
    }
}
