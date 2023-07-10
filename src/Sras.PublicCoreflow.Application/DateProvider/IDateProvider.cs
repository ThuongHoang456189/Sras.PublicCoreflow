using System;

namespace Sras.PublicCoreflow.DateProvider
{
    public interface IDateProvider
    {
        DateTime Today { get; }
        DateTime Now { get; }
    }
}
