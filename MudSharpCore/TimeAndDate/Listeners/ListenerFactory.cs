using MudSharp.Effects.Concrete;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;

#nullable enable
#nullable disable warnings

namespace MudSharp.TimeAndDate.Listeners;

public static class ListenerFactory
{
    public static ITemporalListener CreateDateListener(ICalendar watchCalendar, int watchForDay, string watchForMonth,
        int watchForYear, IMudTimeZone watchForTimeZone, int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference)
    {
        DateListener listener = new(watchCalendar, watchForDay, watchForMonth, watchForYear, watchForTimeZone, repeatTimes,
            payload, objects, debuggerReference);
        watchCalendar.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateDateOffsetListener(ICalendar watchCalendar, int daysOffset,
        int monthsOffset, int yearsOffset, bool ignoreIntercalaries, IMudTimeZone watchForTimeZone, int repeatTimes, Action<object[]> payload,
        object[] objects, string debuggerReference)
    {
        MudDate newDate = new(watchCalendar.CurrentDate);
        newDate.AdvanceDays(daysOffset);
        newDate.AdvanceMonths(monthsOffset, ignoreIntercalaries, true);
        newDate.AdvanceYears(yearsOffset, true);
        DateListener listener = new(watchCalendar, newDate.Day, newDate.Month.Alias, newDate.Year, watchForTimeZone, repeatTimes,
            payload, objects, debuggerReference);
        watchCalendar.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateTimeListener(IClock watchClock, int watchForSecond, int watchForMinute,
        int watchForHour, int repeatTimes, Action<object[]> payload, object[] objects)
    {
        TimeListener listener = new(watchClock, watchForSecond, watchForMinute, watchForHour, repeatTimes,
            payload, objects);
        watchClock.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateTimeOffsetListener(IClock watchClock, int secondsOffset, int minutesOffset,
        int hoursOffset, int repeatTimes, Action<object[]> payload, object[] objects)
    {
        MudTime newTime = MudTime.CopyOf(watchClock.CurrentTime);
        newTime.AddSeconds(secondsOffset);
        newTime.AddMinutes(minutesOffset);
        newTime.AddHours(hoursOffset);
        TimeListener listener = new(watchClock, newTime.Seconds, newTime.Minutes, newTime.Hours, repeatTimes,
            payload, objects);
        watchClock.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateWeekdayListener(ICalendar watchCalendar, List<string> weekdays,
        int repeatTimes, Action<object[]> payload, object[] objects)
    {
        WeekdayListener listener = new(watchCalendar, weekdays, repeatTimes, payload, objects);
        watchCalendar.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateWeekdayTimeListener(IClock watchClock, int watchForSecond,
        int watchForMinute, int watchForHour, ICalendar watchCalendar, List<string> weekdays, int repeatTimes,
        Action<object[]> payload, object[] objects)
    {
        WeekdayTimeListener listener = new(watchClock, watchForSecond, watchForMinute, watchForHour,
            watchCalendar, weekdays, repeatTimes, payload, objects);
        watchCalendar.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener CreateDateTimeOffsetListener(IClock watchClock, int secondsOffset,
        int minutesOffset, int hoursOffset, ICalendar watchCalendar, int daysOffset, int monthsOffset,
        int yearsOffset, bool ignoreIntercalaries, IMudTimeZone watchForTimeZone, int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference)
    {
        MudTime newTime = MudTime.CopyOf(watchClock.CurrentTime);
        newTime.AddSeconds(secondsOffset);
        newTime.AddMinutes(minutesOffset);
        newTime.AddHours(hoursOffset);
        MudDate newDate = new(watchCalendar.CurrentDate);
        newDate.AdvanceDays(daysOffset + newTime.DaysOffsetFromDatum);
        newDate.AdvanceMonths(monthsOffset, ignoreIntercalaries, true);
        newDate.AdvanceYears(yearsOffset, true);

        ITemporalListener listener;
        // If we didn't advance a day, just make a time listener
        if (newDate.Equals(watchCalendar.CurrentDate))
        {
            listener = new TimeListener(watchClock, newTime.Seconds, newTime.Minutes, newTime.Hours, repeatTimes,
                payload, objects);
        }
        else
        {
            listener = new DateListener(watchCalendar, newDate.Day, newDate.Month.Alias, newDate.Year, watchForTimeZone, 0,
                x =>
                    CreateTimeListener(watchClock, newTime.Seconds, newTime.Minutes, newTime.Hours, repeatTimes,
                        payload, objects), objects, debuggerReference
            );
        }

        watchCalendar.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener? CreateDateTimeListener(MudDateTime watchForTime, Action<object[]> payload,
        object[] objects, string debuggerReference)
    {
        if (watchForTime <= watchForTime.Calendar.CurrentDateTime)
        {
            payload(objects);
            return null;
        }

        ITemporalListener listener;
        if (watchForTime.Date.Equals(watchForTime.Calendar.CurrentDate))
        {
            if (watchForTime.Time <= watchForTime.Clock.CurrentTime)
            {
                payload(objects);
                return null;
            }

            listener = new TimeListener(watchForTime.Clock, watchForTime.Time.Seconds, watchForTime.Time.Minutes,
                watchForTime.Time.Hours, 0, payload, objects);
            watchForTime.Gameworld.Add(listener);
            return listener;
        }

        listener = new DateListener(watchForTime, 0, x =>
        {
            if (watchForTime.Time <= watchForTime.Clock.CurrentTime)
            {
                payload(objects);
                return;
            }

            TimeListener timeListener = new(watchForTime.Clock, watchForTime.Time.Seconds,
                watchForTime.Time.Minutes, watchForTime.Time.Hours, 0, payload, objects);
            watchForTime.Gameworld.Add(timeListener);
        }, objects, debuggerReference);
        watchForTime.Gameworld.Add(listener);
        return listener;
    }

    public static ITemporalListener? CreateDateTimeListener(IClock watchClock, int watchForSecond, int watchForMinute,
        int watchForHour, ICalendar watchCalendar, int watchForDay, string watchForMonth, int watchForYear, IMudTimeZone watchForTimeZone,
        int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference)
    {
        ITemporalListener listener;
        if ((watchForDay == -1 && watchForMonth.Length == 0 && watchForYear == -1) ||
            (watchCalendar.CurrentDate.Day == watchForDay &&
             watchCalendar.CurrentDate.Month.Alias == watchForMonth &&
             watchCalendar.CurrentDate.Year == watchForYear))
        {
            if (CheckForAlreadyPassedTime(watchClock, watchForSecond, watchForMinute, watchForHour, payload,
                    objects))
            {
                return null;
            }

            listener = new TimeListener(watchClock, watchForSecond, watchForMinute, watchForHour, repeatTimes,
                payload, objects);
        }
        else
        {
            listener = new DateListener(watchCalendar, watchForDay, watchForMonth, watchForYear, watchForTimeZone, repeatTimes,
                x =>
                {
                    if (CheckForAlreadyPassedTime(watchClock, watchForSecond, watchForMinute, watchForHour, payload,
                            objects))
                    {
                        return;
                    }

                    CreateTimeListener(
                        watchClock, watchForSecond, watchForMinute, watchForHour, 1,
                        payload,
                        objects);
                },
                objects, debuggerReference
            );
        }

        watchClock.Gameworld.Add(listener);
        return listener;
    }

    private static bool CheckForAlreadyPassedTime(IClock watchClock, int watchForSecond, int watchForMinute,
        int watchForHour, Action<object[]> payload, object[] objects)
    {
        MudTime time = watchClock.CurrentTime;
        int timeHours =
            time.Hours +
            (time.Minutes + time.Seconds / watchClock.SecondsPerMinute) /
            watchClock.MinutesPerHour % watchClock.HoursPerDay;
        int timeMinutes =
            (time.Minutes + time.Seconds / watchClock.SecondsPerMinute) %
            watchClock.MinutesPerHour;

        if (timeHours > watchForHour ||
            (timeHours == watchForHour &&
             (timeMinutes > watchForMinute ||
              (timeMinutes == watchForMinute && time.Seconds % watchClock.SecondsPerMinute > watchForSecond)))
           )
        {
            payload(objects);
            return true;
        }

        return false;
    }
}
