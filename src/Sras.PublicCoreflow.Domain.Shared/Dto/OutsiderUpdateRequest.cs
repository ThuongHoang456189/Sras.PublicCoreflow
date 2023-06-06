using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class OutsiderUpdateRequest
    {
        public OutsiderUpdateRequest() { }
        public Guid Id { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        public string? Lastname { get; set; }
        public string? Organization { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
    }
}
