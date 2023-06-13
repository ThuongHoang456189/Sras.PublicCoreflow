using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class CreateEmailTemplateRequest
    {
        public string name { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public Guid conferenceId { get; set; }
        public Guid? trackId { get; set; }
    }
}
