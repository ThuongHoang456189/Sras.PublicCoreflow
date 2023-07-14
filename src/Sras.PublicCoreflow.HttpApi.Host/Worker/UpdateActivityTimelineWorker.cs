using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class UpdateActivityTimelineWorker : HangfireBackgroundWorkerBase
    {
        private readonly ITimezoneProvider _timezoneProvider;
        private readonly IConfiguration _configuration;
        private readonly ISrasBackgroundAppService _srasBackgroundAppService;

        public UpdateActivityTimelineWorker(ITimezoneProvider timezoneProvider, IConfiguration configuration, ISrasBackgroundAppService srasBackgroundAppService)
        {
            _timezoneProvider = timezoneProvider;
            _configuration = configuration;

            var timezone = _timezoneProvider.GetTimeZoneInfo(_configuration["TimeZones:Default"]);

            DateTime seed = DateTime.Parse("2023-07-11T00:00:00");

            seed = DateTime.SpecifyKind(seed, DateTimeKind.Unspecified);

            seed = TimeZoneInfo.ConvertTimeToUtc(seed, timezone);

            RecurringJobId = nameof(UpdateActivityTimelineWorker);
            CronExpression = Cron.Daily(seed.Hour, seed.Minute);
            _srasBackgroundAppService = srasBackgroundAppService;
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken = default)
        {
            using (var uow = LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>().Begin())
            {
                Logger.LogInformation("Executed MyLogWorker..!");

                await _srasBackgroundAppService.UpdateActivityTimelineAsync();
            }
        }
    }
}
