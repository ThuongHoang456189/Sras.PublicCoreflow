using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewingInformationAggregationDto
    {
        public Guid? ConferenceId { get; set; }
        public string? ConferenceFullName { get; set; }
        public string? ConferenceShortName { get; set; }
        public Guid? TrackId { get; set; }
        public string? TrackName { get; set; }
        public Guid? ReviewerId { get; set; }
        public int? Quota { get; set; }
        public List<AggregationSubjectAreaDto>? SubjectAreas { get; set; }
    }
}
