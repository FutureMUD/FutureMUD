using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Intervals;

public static class IntervalExtensions
{
    public static ITemporalListener CreateListenerFromInterval(this RecurringInterval interval,
        MudDateTime referenceTime, Action<object[]> payload,
        object[] objects, string debuggerReference)
    {
        ArgumentNullException.ThrowIfNull(referenceTime);
        return ListenerFactory.CreateDateTimeListener(referenceTime, payload, objects, debuggerReference)!;
    }

    public static ITemporalListener CreateListenerFromInterval(this RecurringInterval interval, ICalendar whichCalendar,
        MudDate referenceDate,
        MudTime recurringTime,
        IMudTimeZone referenceTimezone,
        Action<object[]> payload, object[] objects, string debuggerReference)
    {
        var date = interval.GetNextDate(whichCalendar, referenceDate);
        var time = MudTime.CopyOf(recurringTime, true);
        if (recurringTime.DaysOffsetFromDatum != 0)
        {
            date.AdvanceDays(recurringTime.DaysOffsetFromDatum);
        }

        var target = new MudDateTime(date, time, referenceTimezone);
        return interval.CreateListenerFromInterval(target, payload, objects, debuggerReference);
    }

    public static ITemporalListener CreateRecurringListenerFromInterval(this RecurringInterval interval,
        MudDateTime dateTime,
        Action<object[]> payload, object[] objects, string debuggerReference)
    {
        return CreateRecurringListenerFromInterval(interval, dateTime.Calendar, dateTime.Date, dateTime.Time, dateTime.TimeZone,
            payload, objects, debuggerReference);
    }

    public static ITemporalListener CreateRecurringListenerFromInterval(this RecurringInterval interval,
        ICalendar whichCalendar, MudDate referenceDate,
        MudTime recurringTime, IMudTimeZone referenceTimeZone,
        Action<object[]> payload, object[] objects, string debuggerReference)
    {
        MudDate date = interval.GetNextDate(whichCalendar, referenceDate);
        if (recurringTime.Timezone != whichCalendar.FeedClock.PrimaryTimezone)
        {
            recurringTime = recurringTime.GetTimeByTimezone(whichCalendar.FeedClock.PrimaryTimezone);
            if (recurringTime.DaysOffsetFromDatum != 0)
            {
                referenceDate.AdvanceDays(recurringTime.DaysOffsetFromDatum);
            }
        }

        switch (interval.Type)
        {
            case IntervalType.Minutely:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        MudDate newDate = new(date);
                        MudTime newTime = MudTime.CopyOf(recurringTime).GetTimeByTimezone(referenceTimeZone);
                        newTime.AddMinutes(interval.IntervalAmount);
                        if (newTime.DaysOffsetFromDatum != 0)
                        {
                            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                        }

                        payload(objects1);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, newTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.Hourly:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        MudDate newDate = new(date);
                        MudTime newTime = MudTime.CopyOf(recurringTime).GetTimeByTimezone(referenceTimeZone);
                        newTime.AddHours(interval.IntervalAmount);
                        if (newTime.DaysOffsetFromDatum != 0)
                        {
                            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                        }

                        payload(objects1);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, newTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.Daily:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        MudDate newDate = new(date);
                        newDate.AdvanceDays(interval.IntervalAmount);
                        payload(objects1);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.Monthly:
            case IntervalType.OrdinalDayOfMonth:
            case IntervalType.OrdinalWeekdayOfMonth:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        payload(objects1);
                        MudDate newDate = interval.Type == IntervalType.Monthly
                            ? new MudDate(date)
                            : interval.GetNextDateExclusive(whichCalendar, date);
                        if (interval.Type == IntervalType.Monthly)
                        {
                            newDate.AdvanceMonths(interval.IntervalAmount, true, true);
                        }
                        interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.SpecificWeekday:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        payload(objects1);
                        MudDate newDate = new(date);
                        newDate.AdvanceToNextWeekday(interval.Modifier, interval.IntervalAmount);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.Weekly:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        payload(objects1);
                        MudDate newDate = new(date);
                        newDate.AdvanceDays(interval.IntervalAmount * whichCalendar.Weekdays.Count);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            case IntervalType.Yearly:
                return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
                    recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
                    date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
                    objects1 =>
                    {
                        payload(objects1);
                        MudDate newDate = new(date);
                        newDate.AdvanceYears(interval.IntervalAmount, false);
                        interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
                    }, objects, debuggerReference);
            default:
                throw new NotSupportedException("Unsupported IntervalType in CreateRecurringListenerFromInterval.");
        }
    }
}
