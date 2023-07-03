using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class TemplateResponseDTO
    {
        public List<string>? conferenceHasUsed;

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public NavbarDTO? Navbar { get; set; }
    }
}
