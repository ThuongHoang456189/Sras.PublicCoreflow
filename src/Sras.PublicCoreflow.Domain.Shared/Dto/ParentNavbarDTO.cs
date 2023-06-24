using System.Collections.Generic;

namespace Sras.PublicCoreflow.Dto
{
    public class ParentNavbarDTO
    {
        public string parentLabel { get; set; }
        public string parentId { get; set; }
        public string href { get; set; }
        public List<ChildNavbarDTO> childs { get; set; }
    }
}