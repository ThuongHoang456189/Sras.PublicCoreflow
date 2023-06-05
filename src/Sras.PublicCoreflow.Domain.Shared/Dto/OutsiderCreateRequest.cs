using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class OutsiderCreateRequest
    {
        public OutsiderCreateRequest() { }

        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
    }
}