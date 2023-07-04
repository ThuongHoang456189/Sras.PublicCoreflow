using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class TemplateCreateRequestDTO
    {
        public string name { get; set; }
        public string description { get; set; }
        public List<ParentNavbarDTO> navbar { get; set; }
    }
}
