using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.TimeAndDate.Time
{
    /// <summary>
    ///     Controls the behaviour of Display Time function
    /// </summary>
    public enum TimeDisplayTypes {
        Short,
        Long,
        Immortal,
        Vague,
        Crude
    }

    public delegate void ClockEventHandler();

    public delegate void ClockAdvanceDaysEventHandler(int arg);

    public interface IClock : IHaveMultipleNames, IXmlSavable, IXmlLoadable, IProgVariable, ISaveable
    {
        event ClockEventHandler SecondsUpdated;
        event ClockEventHandler MinutesUpdated;
        event ClockEventHandler HoursUpdated;
        event ClockAdvanceDaysEventHandler DaysUpdated;
        string Alias { get; }
        string Description { get; }
        int SecondsPerMinute { get; }
        int MinutesPerHour { get; }
        int HoursPerDay { get; }
        double InGameSecondsPerRealSecond { get; }
        int HourFixedDigits { get; }
        int MinuteFixedDigits { get; }
        int SecondFixedDigits { get; }
        string ShortDisplayString { get; }
        string LongDisplayString { get; }
        string SuperDisplayString { get; }
        int NumberOfHourIntervals { get; }
        List<string> HourIntervalNames { get; }
        List<string> HourIntervalLongNames { get; }
        bool NoZeroHour { get; }
        MudTime CurrentTime { get; }
        IUneditableAll<IMudTimeZone> Timezones { get; }
        IMudTimeZone PrimaryTimezone { get; }
        void UpdateSeconds();
        void UpdateMinutes();
        void UpdateHours();
        void AdvanceDays(int days);
        void AddTimezone(IMudTimeZone timezone);

        /// <summary>
        ///     Some examples of Display times and their expected outputs:
        ///     "$h$m hours" - 1630 hours
        ///     "$j:$m$i" - 4:30pm
        ///     "$c $l" - half past four in the afternoon
        /// </summary>
        /// <param name="theTime"></param>
        /// <param name="brief"></param>
        /// <returns></returns>
        string DisplayTime(MudTime theTime, string timeString);

        string DisplayTime(MudTime theTime, TimeDisplayTypes type);
        string DisplayTime(TimeDisplayTypes type);
        MudTime GetTime(string timeString);
        void SetTime(MudTime time);
    }
}