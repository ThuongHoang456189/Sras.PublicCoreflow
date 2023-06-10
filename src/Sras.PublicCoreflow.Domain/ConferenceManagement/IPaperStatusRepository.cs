﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IPaperStatusRepository
    {
        Task<IEnumerable<object>> GetAllPaperStatus(Guid? conferenceId);
    }
}
