using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceRole : FullAuditedAggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public bool IsPC { get; private set; }
        public int Factor { get; private set; }

        public ICollection<Incumbent> Incumbents { get; private set; }

        public ConferenceRole(Guid id, string name, bool isPC, int factor) : base(id)
        {
            SetName(name);
            IsPC = isPC;
            Factor = factor;

            Incumbents = new Collection<Incumbent>();
        }

        public ConferenceRole SetName(string name)
        {
            Name = Check.NotNullOrWhiteSpace(string.IsNullOrEmpty(name) ? name : name.Trim(), nameof(name), ConferenceRoleConsts.MaxNameLength);
            return this;
        }
    }
}
