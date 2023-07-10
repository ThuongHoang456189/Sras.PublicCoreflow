using System;

namespace Sras.PublicCoreflow.DateProvider
{
    public class TraversableDateTimeProvider : IDateProvider
    {
        private TraversableDateTimeProvider()
        {
        }

        public static TraversableDateTimeProvider StoppedIn(DateTime moment)
        {
            return new TraversableDateTimeProvider
            {
                Now = moment
            };
        }

        public DateTime Today
        {
            get { return Now.Date; }
        }

        public DateTime Now { get; private set; }

        public void TravelTo(DateTime time)
        {
            Now = time;
        }
    }
}
