using System;
using System.Collections.Generic;
using System.Text;

namespace Sras.PublicCoreflow.Dto
{
    public class RecipientInforForEmail
    {
        public RecipientInforForEmail(string _FirstName, string _LastName, string _FullName, string _Email, string _Organization) {
            FirstName = _FirstName;
            LastName = _LastName;
            FullName = _FullName;
            Email = _Email;
            Organization = _Organization;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }
    }
}
