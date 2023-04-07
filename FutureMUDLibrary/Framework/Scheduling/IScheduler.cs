using System;
using System.Text;

namespace MudSharp.Framework.Scheduling {
    public interface IScheduler {
        void AddSchedule(ISchedule schedule);
        void AddOrDelaySchedule(ISchedule schedule, IFrameworkItem item);
        void CheckSchedules();

        /// <summary>
        ///     Disposes any schedules for whom PertainsTo is true for the IItem
        /// </summary>
        /// <param name="item">The item requested to be disposed</param>
        void Destroy(IFrameworkItem item);

        /// <summary>
        ///     Disposes any schedules for whom PertainsTo is true for the IItem and is also of ScheduleType type
        /// </summary>
        /// <param name="item">The item requested to be disposed</param>
        /// <param name="type">The ScheduleType requested to be destroyed</param>
        void Destroy(IFrameworkItem item, ScheduleType type);

        /// <summary>
        ///     Pushes the fire time for a schedule back by the specified TimeSpan
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="delay"></param>
        void DelaySchedule(ISchedule schedule, TimeSpan delay);

        void DelayScheduleType(IFrameworkItem item, ScheduleType type, TimeSpan delay);

        TimeSpan RemainingDuration(IFrameworkItem item, ScheduleType type);
        TimeSpan OriginalDuration(IFrameworkItem item, ScheduleType type);
        void DebugOutputForScheduler(StringBuilder sb);
    }
}