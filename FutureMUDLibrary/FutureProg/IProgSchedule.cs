using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;

namespace MudSharp.FutureProg
{
    public interface IProgSchedule : ISaveable, IFrameworkItem
    {
        RecurringInterval Interval { get; set; }
        MudDateTime NextReferenceTime { get; set; }
        IFutureProg Prog { get; set; }
        void SchedulePayload(object[] parameters);
        void Delete();
    }
}