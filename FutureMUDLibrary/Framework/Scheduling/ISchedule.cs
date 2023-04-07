using System;

namespace MudSharp.Framework.Scheduling
{
    public interface ISchedule
    {
        ScheduleType Type { get; }
        DateTime CreatedAt { get; }
        TimeSpan Duration { get; }
        DateTime TriggerETA { get; set; }
        string DebugInfoString { get; }
        void Fire();
        bool PertainsTo(IFrameworkItem item);
        bool PertainsTo(IFrameworkItem item, ScheduleType type);
    }
}