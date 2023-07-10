using System;

namespace Sras.PublicCoreflow.DateProvider
{
    public class SystemDate
    {
        private static IDateProvider provider;

        public static void Use(IDateProvider dateProvider)
        {
            provider = dateProvider;
        }
        static SystemDate()
        {
            UseDefault();
        }
        public static DateTime Today
        {
            get
            {
                return provider.Today;
            }
        }
        public static DateTime Now
        {
            get
            {
                return provider.Now;
            }
        }

        public static void UseDefault()
        {
            provider = new DateTimeProvider();
        }
    }
}
