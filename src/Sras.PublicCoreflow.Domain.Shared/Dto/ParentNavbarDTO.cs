using System.Collections.Generic;

namespace Sras.PublicCoreflow.Dto
{
    public class ParentNavbarDTO
    {
        public string labelCha { get; set; }
        public string idCha { get; set; }
        public List<ChildNavbarDTO> listBarsCon { get; set; }
    }
}