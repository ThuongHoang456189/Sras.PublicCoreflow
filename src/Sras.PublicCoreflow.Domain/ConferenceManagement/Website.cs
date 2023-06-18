using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class Website : FullAuditedAggregateRoot<Guid>
    {
        [ForeignKey(nameof(Conference))]
        public override Guid Id { get; protected set; }
        public Conference Conference { get; private set; }
        public string? NavBar { get; set; } // {home, conference: [conference-1, conference-2]}
        public string? Pages { get; set; } // list of HTML pages e.g: conference/conference-1.html; conference/conference-2.html
        public string? RootFilePath { get; set; } // Location of parent folder for files to finally export
        public string? TempFilePath { get; set; } // Location of parent folder for temp files to load into text editor
        public Guid WebTemplateId { get; private set; }
        public WebTemplate WebTemplate { get; set; }

        public Website(Guid id, string? navBar, string? pages, string? rootFilePath, string? tempFilePath, Guid webTemplateId) : base(id) 
        {
            NavBar = navBar;
            Pages = pages;
            RootFilePath = rootFilePath;
            TempFilePath = tempFilePath;
            WebTemplateId = webTemplateId;
        }
    }
}
