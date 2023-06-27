using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class FileNameContentRequest
    {
        public string fileName { get; set; }
        public string tempContent { get; set; }
        public string finalContent { get; set; }
    }
}
