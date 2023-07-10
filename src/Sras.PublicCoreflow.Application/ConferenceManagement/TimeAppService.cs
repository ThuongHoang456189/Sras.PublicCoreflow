using System;
using Volo.Abp.Timing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TimeAppService : PublicCoreflowAppService, ITimeAppService
    {
        private readonly IClock _clock;
        private readonly ITimezoneProvider _timezoneProvider;
        public TimeAppService(IClock clock, ITimezoneProvider timezoneProvider)
        {
            _clock = clock;
            
            _timezoneProvider = timezoneProvider;
        }

        public string GetTimeZone()
        {
            try
            {
                
                var timezone = _timezoneProvider.GetTimeZoneInfo("Asia/Ho_Chi_Minh");

                var newdate = TimeZoneInfo.ConvertTimeFromUtc(_clock.Now, timezone);

                var tnow = _clock.Now;
                
                return tnow.ToString();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            return "Khong biet";
        }
    }
}
