using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewingSubmissionRelatedFilesDto
    {
        public List<string>? SubmissionFiles { get; set; }
        public List<string>? SupplementaryMaterialFiles { get; set; }
        public int? RevisionNo { get; set; }
        public List<string>? RevisionFiles { get; set; }
    }
}