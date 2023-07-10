using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Publication
    {
        public Guid PublicationId { get; set; }
        public string PublicationName { get; set; } // ten bai bao
        public string Publisher { get; set; } // nha xuat ban
        public DateTime PublicationDate { get; set; }
        public List<ReferenceWithReferenceTypeInclusion> PublicationLinks { get; set; } // -with publication-link json, bat buoc co link doi
        public int WorkTypeId { get; set; } // -ReferenceId
        public string WorkType { get; set; } // -ReferenceTypeName -work-type json
        public List<string> Contributors { get; set; }
        public bool IsLeadAuthor { get; set; }
    }
}
