using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class PaperStatusCreateRequest
    {
        public PaperStatusCreateRequest() { }

        public Guid? ConferenceId { get; set; }
        public string Text { get; set; }
        public bool VisibleToAuthor { get; set; }
    }
}
