namespace Sras.PublicCoreflow.ConferenceManagement
{
    public static class TrackConsts
    {
        public const int MaxNameLength = 50;
        public const int MaxSubmissionInstructionLength = 2000;

        public static readonly SubjectAreaRelevanceCoefficients DefaultSubjectAreaRelevanceCoefficients = new SubjectAreaRelevanceCoefficients
        {
            pp = 0.8,
            ps = 0.16,
            sp = 0.16,
            ss = 0.04
        };
    }
}
