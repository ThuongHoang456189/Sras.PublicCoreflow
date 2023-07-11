namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionSummaryAuthorDto
    {
        public string? AuthorEmail { get; set; }
        public string? AuthorNamePrefix { get; set; }
        public string? AuthorFullName { get; set; }
        public string? AuthorOrganization { get; set; }
        public bool? HasAccount { get; set; }
        public bool? IsPrimaryContact { get; set; }
    }
}
