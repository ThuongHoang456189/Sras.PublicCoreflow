using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class WebTemplate : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string RootFilePath { get; set; }

        public ICollection<Website> Websites { get; set; }

        public WebTemplate(Guid id, string name, string description, string rootFilePath) : base(id)
        {
            Name = name;
            Description = description;

            RootFilePath = rootFilePath;

            Websites = new Collection<Website>();
        }
    }
}
