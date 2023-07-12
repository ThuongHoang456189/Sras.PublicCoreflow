using System;
using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Publication
    {
        public Guid publicationId { get; set; }
        public string publicationName { get; set; } // ten bai bao
        public string publisher { get; set; } // nha xuat ban
        public DateTime publicationDate { get; set; }
        public string doi { get; set; }
        public string dOILink { get; set; }
        public List<PublicationLink> publicationLinks { get; set; } // -with publication-link json, bat buoc co link doi
        public int workTypeId { get; set; } // -ReferenceId -work-type json
        public List<string> contributors { get; set; }
        public bool isLeadAuthor { get; set; }
    }
}
