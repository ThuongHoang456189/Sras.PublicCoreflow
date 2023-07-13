using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionRelatedFilesDto
    {
        public List<string>? SubmissionFiles { get; set; }
        public List<string>? SupplementaryMaterialFiles { get; set; }
        public int? RevisionNo { get; set; }
        public List<string>? RevisionFiles { get; set; }
        public List<string>? CameraReadyFiles { get; set; }
        public List<string>? CopyRightFiles { get; set; }
        public List<string>? PresentationFiles { get; set; }
    }
}