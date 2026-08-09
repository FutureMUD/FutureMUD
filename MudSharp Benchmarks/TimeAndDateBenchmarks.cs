using BenchmarkDotNet.Attributes;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using System.Xml.Linq;

namespace MudSharp_Benchmarks;

[MemoryDiagnoser]
public sealed class TimeAndDateBenchmarks
{
	private Calendar _calendar = null!;
	private Clock _clock = null!;
	private MudDateTime _oldReference = null!;
	private RecurringInterval _dailyInterval = null!;
	private MudTimeSpan _mixedSpan = null!;

	[GlobalSetup]
	public void Setup()
	{
		_clock = new Clock(XElement.Parse(@"<Clock><Alias>utc</Alias><Description>UTC</Description><ShortDisplayString>$j:$m:$s</ShortDisplayString><SuperDisplayString>$j:$m:$s</SuperDisplayString><LongDisplayString>$j:$m:$s</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1.5</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>2</HourFixedDigits><NoZeroHour>false</NoZeroHour><NumberOfHourIntervals>2</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>am</HourIntervalName><HourIntervalName>pm</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>morning</HourIntervalLongName><HourIntervalLongName>afternoon</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals /></Clock>"));
		_calendar = new Calendar();
		_calendar.SetupTestData();
		_calendar.FeedClock = _clock;
		_calendar.SetDate("1/jan/12026");
		_clock.SetTime(MudTime.CreatePrimaryTime(0, 0, 0, _clock.PrimaryTimezone, _clock));
		_oldReference = new MudDateTime(_calendar.GetDate("1/jan/2026"),
			MudTime.FromLocalTime(0, 0, 0, _clock.PrimaryTimezone, _clock), _clock.PrimaryTimezone);
		_dailyInterval = new RecurringInterval { Type = IntervalType.Daily, IntervalAmount = 1 };
		_mixedSpan = new MudTimeSpan(2, -3, 4, -5, 6, -7, 8, -9);
	}

	[Benchmark]
	public int DistantYearDifference()
	{
		return _calendar.CountDaysBetweenYears(2010, 12026);
	}

	[Benchmark]
	public Year ReusedGeneratedYear()
	{
		return _calendar.CreateYear(12026);
	}

	[Benchmark]
	public MudDateTime LongGapDailyRecurrence()
	{
		return _dailyInterval.GetNextDateTime(_oldReference);
	}

	[Benchmark]
	public string ClockDisplay()
	{
		return _clock.DisplayTime(_clock.CurrentTime, TimeDisplayTypes.Short);
	}

	[Benchmark]
	public MudTimeSpan StructuralSpanRoundTrip()
	{
		return MudTimeSpan.Parse(_mixedSpan.GetRoundTripParseText);
	}
}
