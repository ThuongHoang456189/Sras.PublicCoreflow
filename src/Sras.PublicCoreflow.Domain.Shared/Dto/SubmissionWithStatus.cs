using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class SubmissionWithStatus
    {
        public SubmissionWithStatus() { }
        public string SubmissionId { get; set; }
        public string StatusId { get; set; }
        public bool isAllAuthor { get; set; }
    }
}
