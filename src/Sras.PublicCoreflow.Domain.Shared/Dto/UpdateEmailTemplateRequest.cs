using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class UpdateEmailTemplateRequest
    {
        public string templateName { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public Guid templateId { get; set; }
    }
}
