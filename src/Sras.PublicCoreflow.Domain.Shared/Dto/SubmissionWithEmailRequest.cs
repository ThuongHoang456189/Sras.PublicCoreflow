using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class SubmissionWithEmailRequest
    {
        public Guid trackId { get; set; }
        public List<Guid> paperStatuses { get; set; }
        public bool allAuthors { get; set; }
    }
}
