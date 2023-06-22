namespace Sras.PublicCoreflow.ConferenceManagement
{
    public enum LoginResultType : byte
    {
        Success = 1,

        InvalidUserNameOrPassword = 2,

        NotAllowed = 3,

        LockedOut = 4,
    }
}
