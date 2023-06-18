using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class WebTemplate : FullAuditedAggregateRoot<Guid>
    {
        public string RootFilePath { get; set; }

        public ICollection<Website> Websites { get; set; }

        public WebTemplate(Guid id, string rootFilePath) : base(id)
        {
            RootFilePath = rootFilePath;

            Websites = new Collection<Website>();
        }
    }
}
