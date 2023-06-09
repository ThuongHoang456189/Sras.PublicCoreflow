using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class SubmissionWithEmailRequest
    {
        public Guid TrackId { get; set; }
        public List<Guid> PaperStatuses { get; set; }
        public bool AllAuthors { get; set; }
    }
}
