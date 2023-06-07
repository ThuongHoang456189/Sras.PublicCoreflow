using System;
using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceListFilterDto : PagedAndSortedResultRequestDto
    {
        public string? InclusionText { get; set; }
        public string? FullName { get; set; }
        public string? ShortName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAccepted { get; set; }
        public Guid? AccountId { get; set; }
    }
}
