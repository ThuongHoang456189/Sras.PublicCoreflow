using System;

namespace Sras.PublicCoreflow.Extension
{
    public static class DateTimeExtensions
    {
        public static DateTime GetToday()
        {
            return DateTime.Now.Date;
        }

        public static DateTime MinDate(DateTime date1, DateTime date2)
        {
            return date1.Date < date2.Date ? date1.Date : date2.Date;
        }

        public static DateTime MaxDate(DateTime date1, DateTime date2)
        {
            return date1.Date > date2.Date ? date1.Date : date2.Date;
        }

        public static bool IsLessThan(this DateTime date1, DateTime date2)
        {
            return date1.Date < date2.Date ? true : false;
        }

        public static bool IsGreaterThan(this DateTime date1, DateTime date2)
        {
            return date1.Date > date2.Date ? true : false;
        }
    }
}
