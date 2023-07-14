﻿using System;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorSubmissionAggregationInput
    {
        public string? InclusionText { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid? TrackId { get; set; }
        public Guid AccountId { get; set; }
        public Guid? StatusId { get; set; }
        public string? Sorting { get; set; }
        public bool? SortedAsc { get; set; }
        public int? SkipCount { get; set; }
        public int? MaxResultCount { get; set; }
    }
}