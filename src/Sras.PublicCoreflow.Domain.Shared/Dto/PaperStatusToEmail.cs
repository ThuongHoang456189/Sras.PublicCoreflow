﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class PaperStatusToEmail
    {
        public Guid userId { get; set; }
        public Guid trackId { get; set; }
        public List<PaperStatusIdAndTemplateId> statuses { get; set; }
        public bool allAuthors { get; set; }

    }

}
