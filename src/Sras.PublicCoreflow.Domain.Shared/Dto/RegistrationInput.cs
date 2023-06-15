using System.Collections.Generic;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class RegistrationInput
    {
        public RegistrationPaperInput MainPaper { get; set; } = new RegistrationPaperInput();
        public List<RegistrationPaperInput> ExtraPapers { get; set; } = new List<RegistrationPaperInput>();
    }
}