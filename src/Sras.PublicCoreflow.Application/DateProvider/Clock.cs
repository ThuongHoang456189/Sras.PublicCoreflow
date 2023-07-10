using Microsoft.Extensions.Options;
using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;

namespace Sras.PublicCoreflow.DateProvider
{
    public class Clock : IClock, ITransientDependency
    {
        protected AbpClockOptions Options { get; }

        public Clock(IOptions<AbpClockOptions> options)
        {
            Options = options.Value;
        }

        public virtual DateTime Now => Options.Kind == DateTimeKind.Utc ? DateTime.UtcNow.AddMonths(10) : DateTime.Now.AddMonths(10);

        public virtual DateTimeKind Kind => Options.Kind;

        public virtual bool SupportsMultipleTimezone => Options.Kind == DateTimeKind.Utc;

        public virtual DateTime Normalize(DateTime dateTime)
        {
            if (Kind == DateTimeKind.Unspecified || Kind == dateTime.Kind)
            {
                return dateTime.AddMonths(10);
            }

            if (Kind == DateTimeKind.Local && dateTime.Kind == DateTimeKind.Utc)
            {
                return dateTime.ToLocalTime().AddMonths(10);
            }

            if (Kind == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime().AddMonths(10);
            }

            return DateTime.SpecifyKind(dateTime, Kind);
        }
    }
}
