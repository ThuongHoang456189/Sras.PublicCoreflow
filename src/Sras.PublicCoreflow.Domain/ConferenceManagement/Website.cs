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
        public string? NavBar { get; set; }
        public string? Pages { get; set; }
        public string? RootFilePath { get; set; }
        public Guid WebTemplateId { get; private set; }
        public WebTemplate WebTemplate { get; private set; }

        public Website(Guid id, string? navBar, string? pages, string? rootFilePath, Guid webTemplateId) : base(id) 
        {
            NavBar = navBar;
            Pages = pages;
            RootFilePath = rootFilePath;
            WebTemplateId = webTemplateId;
        }
    }
}
