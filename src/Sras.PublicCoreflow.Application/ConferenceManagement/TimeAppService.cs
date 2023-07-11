using Microsoft.Extensions.Configuration;
using Sras.PublicCoreflow.DateProvider;
using System;
using Volo.Abp.Timing;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class TimeAppService : PublicCoreflowAppService, ITimeAppService
    {
        private readonly IClock _clock;
        private readonly ITimezoneProvider _timezoneProvider;
        private readonly IConfiguration _configuration;
        public TimeAppService(IClock clock, ITimezoneProvider timezoneProvider, IConfiguration configuration)
        {
            _clock = clock;
            _timezoneProvider = timezoneProvider;
            _configuration = configuration;
        }

        public DateTime GetNow()
        {
            var timezone = _timezoneProvider.GetTimeZoneInfo(_configuration["TimeZones:Default"]);

            return TimeZoneInfo.ConvertTimeFromUtc(_clock.Now, timezone);
        }

        public DateTime SetNow(DateTime now)
        {
            var timezone = _timezoneProvider.GetTimeZoneInfo(_configuration["TimeZones:Default"]);

            now = DateTime.SpecifyKind(now, DateTimeKind.Unspecified);

            var UTCNow = TimeZoneInfo.ConvertTimeToUtc(now, timezone);

            SimulationDateTimeOffset.OffSetFromNow = UTCNow.ToUniversalTime() - DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(_clock.Now, timezone);
        }

        public DateTime Reset()
        {
            var timezone = _timezoneProvider.GetTimeZoneInfo(_configuration["TimeZones:Default"]);

            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            var UTCNow = TimeZoneInfo.ConvertTimeToUtc(now, timezone);

            SimulationDateTimeOffset.OffSetFromNow = UTCNow.ToUniversalTime() - DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeFromUtc(_clock.Now, timezone);
        }
    }
}
