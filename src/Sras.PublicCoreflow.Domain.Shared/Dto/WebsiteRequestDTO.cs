using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class WebsiteRequestDTO
    {
        public Guid? webId { get; set; }
        public string? navbar { get; set; }
        public string? pages { get; set; }
        public string? tempPath { get; set; }
        public string? finalPath { get; set; }
        public Guid? webTemplateId { get; set; }
    }
}
