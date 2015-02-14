using System;

namespace SoftwareMind.Scheduler
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}