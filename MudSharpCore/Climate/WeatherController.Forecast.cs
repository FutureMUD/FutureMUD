#nullable enable

using System.Text.Json;
using MudSharp.Celestial;

namespace MudSharp.Climate;

public partial class WeatherController : IWeatherForecastSource
{
	private sealed class ScheduleState
	{
		public int Version { get; set; } = 2;
		public long Minute { get; set; }
		public ulong RandomState { get; set; }
		public int Interval { get; set; }
		public int MinuteCounter { get; set; }
		public WeatherForecastPoint? Current { get; set; }
		public List<WeatherForecastPoint> Points { get; set; } = [];
	}

	private readonly List<WeatherForecastPoint> _forecast = [];
	private WeatherRandom _forecastRandom = new((ulong)Random.Shared.NextInt64());
	private long _forecastMinute;
	private int _forecastInterval;
	private bool _forecastTimeSubscribed;
	public int ForecastHorizonDays { get; private set; } = 7;
	public long ForecastMinute => _forecastMinute;
	private int MinutesPerDay => checked(FeedClock.HoursPerDay * FeedClock.MinutesPerHour);
	public long ForecastDay => (long)Math.Floor((_forecastMinute +
		(FeedClockTimeZone.OffsetHours - FeedClock.PrimaryTimezone.OffsetHours) * FeedClock.MinutesPerHour +
		FeedClockTimeZone.OffsetMinutes - FeedClock.PrimaryTimezone.OffsetMinutes) / (double)MinutesPerDay);
	private int PrimaryMinute => FeedClock.CurrentTime.Hours * FeedClock.MinutesPerHour + FeedClock.CurrentTime.Minutes;
	private int LocalMinute
	{
		get
		{
			var time = FeedClock.CurrentTime.GetTimeByTimezone(FeedClockTimeZone);
			return time.Hours * FeedClock.MinutesPerHour + time.Minutes;
		}
	}

	private void LoadForecast(string? json, int horizon)
	{
		ForecastHorizonDays = horizon is >= 1 and <= 30 ? horizon : 7;
		var calendar = Gameworld.Calendars.Where(x => x.FeedClock == FeedClock).OrderBy(x => x.Id).FirstOrDefault();
		_forecastMinute = calendar is not null ? calendar.CurrentInstant.Ticks / FeedClock.SecondsPerMinute : PrimaryMinute;
		if (!string.IsNullOrWhiteSpace(json))
		{
			try
			{
				var state = JsonSerializer.Deserialize<ScheduleState>(json);
				if (state is { Version: 2 })
				{
					_forecastRandom = new(state.RandomState);
					if (calendar is null) _forecastMinute = state.Minute;
					_forecastInterval = state.Interval;
					if (_forecastMinute == state.Minute && state.MinuteCounter == MinuteCounter && state.Current is { } current &&
						current.EventId == CurrentWeatherEvent.Id && current.SeasonId == CurrentSeason.Id &&
						current.TemperatureFluctuation == CurrentTemperatureFluctuation && current.UnchangedPeriods == ConsecutiveUnchangedPeriods &&
						state.Points is { Count: <= 200_000 } && state.Interval == Math.Max(1, RegionalClimate.ClimateModel.MinuteProcessingInterval) &&
						state.Points.All(x => x is not null && x.Minute > _forecastMinute && double.IsFinite(x.TemperatureFluctuation) &&
							x.UnchangedPeriods >= 0 && x.LocalHour >= 0 && x.LocalHour < FeedClock.HoursPerDay && Enum.IsDefined(x.TimeOfDay) &&
							Gameworld.WeatherEvents.Get(x.EventId) is not null && RegionalClimate.Seasons.Any(s => s.Id == x.SeasonId)) &&
						(state.Points.Count == 0 || state.Points[0].Minute == _forecastMinute + Math.Max(1, state.Interval - MinuteCounter)) &&
						state.Points.Zip(state.Points.Skip(1)).All(x => x.Second.Minute - x.First.Minute == state.Interval))
						_forecast.AddRange(state.Points);
				}
			}
			catch (JsonException)
			{
				// An unusable queue is rebuilt from the durable current weather, never replayed.
			}
		}
		SubscribeForecastClock();
	}

	private void SubscribeForecastClock()
	{
		if (_forecastTimeSubscribed) return;
		FeedClock.TimeChanged += ResynchroniseForecast;
		_forecastTimeSubscribed = true;
	}

	internal void ResynchroniseForecast()
	{
		using var exposureChange = MudSharp.Form.Material.EnvironmentalExposureService.ChangingWeather(Gameworld, this);
		var calendar = Gameworld.Calendars.Where(x => x.FeedClock == FeedClock).OrderBy(x => x.Id).FirstOrDefault();
		if (calendar is not null) _forecastMinute = calendar.CurrentInstant.Ticks / FeedClock.SecondsPerMinute;
		InvalidateForecast();
		UpdateCurrentSeason();
		CalculateCurrentTemperature();
	}

	public void InvalidateForecast()
	{
		_forecast.Clear();
		Changed = true;
	}

	private string SaveForecast() => JsonSerializer.Serialize(new ScheduleState
	{
		Minute = _forecastMinute, RandomState = _forecastRandom.State,
		Interval = _forecastInterval, Points = _forecast, Current = CurrentPoint(), MinuteCounter = MinuteCounter
	});

	private WeatherForecastPoint CurrentPoint() => new(_forecastMinute, CurrentWeatherEvent.Id, CurrentSeason.Id,
		CurrentTemperatureFluctuation, ConsecutiveUnchangedPeriods,
		LocalMinute / FeedClock.MinutesPerHour, Celestial?.CurrentTimeOfDay(GeographyForTimeOfDay) ?? TimeOfDay.Night);

	public IReadOnlyList<WeatherForecastPoint> GetForecast()
	{
		EnsureForecast();
		var end = _forecastMinute + (long)ForecastHorizonDays * MinutesPerDay;
		var checkpoints = new[] { CurrentPoint() }.Concat(_forecast.Where(x => x.Minute <= end)).ToArray();
		var samples = new SortedDictionary<long, WeatherForecastPoint>(checkpoints.ToDictionary(x => x.Minute));
		var index = 0;
		// Conditions persist between processing checkpoints. Sample every local hour as well,
		// including the end of the horizon, so slow climates still have a full temperature/daylight outlook.
		for (var minute = _forecastMinute; minute <= end; minute = Math.Min(end, minute + FeedClock.MinutesPerHour))
		{
			while (index + 1 < checkpoints.Length && checkpoints[index + 1].Minute <= minute) index++;
			if (!samples.ContainsKey(minute))
			{
				var offset = minute - _forecastMinute;
				samples[minute] = checkpoints[index] with { Minute = minute,
					LocalHour = (int)((LocalMinute + offset) % MinutesPerDay) / FeedClock.MinutesPerHour,
					TimeOfDay = WeatherAstronomy.Sample(Celestial, SecondsUntil(offset), GeographyForTimeOfDay).Time };
			}
			if (minute == end) break;
		}
		return samples.Values.ToArray();
	}

	private double SecondsUntil(long minuteOffset) =>
		(minuteOffset * (double)FeedClock.SecondsPerMinute - FeedClock.CurrentTime.Seconds) / FeedClock.InGameSecondsPerRealSecond;

	private void EnsureForecast(bool processingTick = false)
	{
		var interval = Math.Max(1, RegionalClimate.ClimateModel.MinuteProcessingInterval);
		if (_forecastInterval != interval) { _forecast.Clear(); _forecastInterval = interval; }
		var horizon = checked((long)ForecastHorizonDays * MinutesPerDay);
		if (horizon / interval > 200_000)
		{
			if (!processingTick) throw new InvalidOperationException("This forecast horizon exceeds 200,000 climate checkpoints; reduce its days or increase the climate processing interval.");
			// Unusual clocks or later interval edits must not disable live weather progression.
			// Process the due checkpoint without allocating an oversized prediction queue.
			horizon = 0;
		}
		var end = checked(_forecastMinute + horizon);
		var point = _forecast.LastOrDefault() ?? CurrentPoint();
		var nextMinute = _forecast.Count == 0 ? _forecastMinute + Math.Max(processingTick ? 0 : 1, interval - MinuteCounter) : point.Minute + interval;
		while (nextMinute <= end)
		{
			var offset = nextMinute - _forecastMinute;
			var sky = WeatherAstronomy.Sample(Celestial, SecondsUntil(offset), GeographyForTimeOfDay);
			var year = RegionalClimate.SeasonRotation.Ceiling > 0 ? RegionalClimate.SeasonRotation.Ceiling : Celestial?.CelestialDaysPerYear ?? 365.0;
			var season = RegionalClimate.SeasonRotation.Ranges.Any()
				? RegionalClimate.SeasonRotation.Get(WeatherClimateUtilities.ApplySeasonPhaseShift(sky.Day, year, OppositeHemisphere)) : CurrentSeason;
			var previous = Gameworld.WeatherEvents.Get(point.EventId);
			var drift = WeatherFrozen ? point.TemperatureFluctuation : WeatherClimateUtilities.AdvanceTemperatureFluctuation(
				point.TemperatureFluctuation, RegionalClimate.TemperatureFluctuationStandardDeviation,
				RegionalClimate.TemperatureFluctuationPeriod, interval, _forecastRandom.NextDouble);
			var weather = WeatherFrozen ? previous : RegionalClimate.ClimateModel.HandleWeatherTick(previous, season, sky.Time, point.UnchangedPeriods, _forecastRandom.NextDouble) ?? previous;
			var localHour = (int)(((LocalMinute + offset) % MinutesPerDay + MinutesPerDay) % MinutesPerDay) / FeedClock.MinutesPerHour;
			point = new(nextMinute, weather.Id, season.Id, drift, weather == previous ? point.UnchangedPeriods + 1 : 0, localHour, sky.Time);
			_forecast.Add(point);
			nextMinute += interval;
			Changed = true;
		}
	}

	private WeatherForecastPoint TakeScheduledWeather()
	{
		EnsureForecast(true);
		var point = _forecast[0];
		_forecast.RemoveAt(0);
		return point;
	}

	private bool BuildingCommandForecast(ICharacter actor, StringStack command)
	{
		if (!int.TryParse(command.PopSpeech(), out var days) || days is < 1 or > 30)
		{
			actor.OutputHandler.Send("Forecast horizons must be between 1 and 30 game days.");
			return false;
		}
		if ((long)days * MinutesPerDay / Math.Max(1, RegionalClimate.ClimateModel.MinuteProcessingInterval) > 200_000)
		{
			actor.OutputHandler.Send("That horizon would exceed 200,000 weather checkpoints. Choose fewer days or increase the climate processing interval.");
			return false;
		}
		ForecastHorizonDays = days;
		// Retain an already-issued future when shrinking, so expanding again cannot reroll it.
		Changed = true;
		actor.OutputHandler.Send($"This controller now forecasts {days.ToStringN0(actor).ColourValue()} game days ahead.");
		return true;
	}
}
