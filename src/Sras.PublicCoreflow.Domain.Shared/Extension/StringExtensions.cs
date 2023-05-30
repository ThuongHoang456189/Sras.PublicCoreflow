namespace Sras.PublicCoreflow.Extension
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str, string otherStr)
        {
            return string.Equals(str.ToLower(), otherStr.ToLower());
        }
    }
}
