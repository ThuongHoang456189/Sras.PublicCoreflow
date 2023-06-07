using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class OutsiderCreateResponse
    {
        public OutsiderCreateResponse() { }
        public string OutsiderId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Organization { get; set; }
        public string Email { get; set; }
        public bool hasAccount { get; set; }
        public List<string> ParticipantIds { get; set; }
    }
}