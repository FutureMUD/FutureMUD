﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		if (referenceTime.TimeZone != referenceTime.Clock.PrimaryTimezone)
		{
			referenceTime = referenceTime.GetByTimeZone(referenceTime.Clock.PrimaryTimezone);
		}

		switch (interval.Type)
		{
			case IntervalType.Minutely:
			case IntervalType.Hourly:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Daily:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Monthly:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			case IntervalType.SpecificWeekday:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Weekly:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Yearly:
				return ListenerFactory.CreateDateTimeListener(referenceTime.Clock,
					referenceTime.Time.Seconds, referenceTime.Time.Minutes, referenceTime.Time.Hours,
					referenceTime.Calendar,
					referenceTime.Date.Day, referenceTime.Date.Month.Alias, referenceTime.Date.Year, referenceTime.TimeZone, 1,
					payload, objects, debuggerReference);
			default:
				throw new NotSupportedException("Unsupported IntervalType in CreateListenerFromInterval.");
		}
	}

	public static ITemporalListener CreateListenerFromInterval(this RecurringInterval interval, ICalendar whichCalendar,
		MudDate referenceDate,
		MudTime recurringTime,
		IMudTimeZone referenceTimezone,
		Action<object[]> payload, object[] objects, string debuggerReference)
	{
		var date = interval.GetNextDate(whichCalendar, referenceDate);
		switch (interval.Type)
		{
			case IntervalType.Minutely:
			case IntervalType.Hourly:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Daily:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Monthly:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			case IntervalType.SpecificWeekday:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Weekly:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			case IntervalType.Yearly:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimezone, 1,
					payload, objects, debuggerReference);
			default:
				throw new NotSupportedException("Unsupported IntervalType in CreateListenerFromInterval.");
		}
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
		var date = interval.GetNextDate(whichCalendar, referenceDate);
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
						var newDate = new MudDate(date);
						var newTime = new MudTime(recurringTime).GetTimeByTimezone(referenceTimeZone);
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
						var newDate = new MudDate(date);
						var newTime = new MudTime(recurringTime).GetTimeByTimezone(referenceTimeZone);
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
						var newDate = new MudDate(date);
						newDate.AdvanceDays(interval.IntervalAmount);
						payload(objects1);
						interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
					}, objects, debuggerReference);
			case IntervalType.Monthly:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
					objects1 =>
					{
						payload(objects1);
						var newDate = new MudDate(date);
						newDate.AdvanceMonths(interval.IntervalAmount, true, true);
						interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
					}, objects, debuggerReference);
			case IntervalType.SpecificWeekday:
				return ListenerFactory.CreateDateTimeListener(whichCalendar.FeedClock,
					recurringTime.Seconds, recurringTime.Minutes, recurringTime.Hours, whichCalendar,
					date.Day, date.Month.Alias, date.Year, referenceTimeZone, 1,
					objects1 =>
					{
						payload(objects1);
						var newDate = new MudDate(date);
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
						var newDate = new MudDate(date);
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
						var newDate = new MudDate(date);
						newDate.AdvanceYears(interval.IntervalAmount, false);
						interval.CreateListenerFromInterval(whichCalendar, newDate, recurringTime, referenceTimeZone, payload, objects1, debuggerReference);
					}, objects, debuggerReference);
			default:
				throw new NotSupportedException("Unsupported IntervalType in CreateRecurringListenerFromInterval.");
		}
	}
}