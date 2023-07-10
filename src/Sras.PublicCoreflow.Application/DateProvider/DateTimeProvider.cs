using System;

namespace Sras.PublicCoreflow.DateProvider
{
    public class DateTimeProvider : IDateProvider
    {
        public DateTime Today
        {
            get { return DateTime.Today; }
        }

        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
