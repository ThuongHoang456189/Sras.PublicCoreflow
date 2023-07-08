namespace Sras.PublicCoreflow.ConferenceManagement
{
    public enum LoginResultType : byte
    {
        Success = 1,

        InvalidUserNameOrPassword = 2,

        EmailNotConfirmed = 3,

        NotAllowed = 4,

        LockedOut = 5,
    }
}
