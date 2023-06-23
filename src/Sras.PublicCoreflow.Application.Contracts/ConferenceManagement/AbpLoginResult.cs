namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AbpLoginResult
    {
        public AbpLoginResult(LoginResultType result)
        {
            Result = result;
        }

        public LoginResultType Result { get; }

        public string Description => Result.ToString();
    }
}
