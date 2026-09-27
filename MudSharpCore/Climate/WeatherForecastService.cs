#nullable enable

using System.Globalization;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework.Units;
using MudSharp.RPG.Checks;

namespace MudSharp.Climate;

internal static class WeatherForecastService
{
	internal static bool CanObserve(ICharacter actor) => !actor.Location.IsUnderwaterLayer(actor.RoomLayer) &&
		actor.Location.OutdoorsType(actor) is CellOutdoorsType.Outdoors or CellOutdoorsType.IndoorsWithWindows or CellOutdoorsType.IndoorsClimateExposed &&
		actor.CanSee(actor.Location);

	internal static void Show(ICharacter actor, bool table)
	{
		if (actor.Location.WeatherController is not IWeatherController controller || controller is not IWeatherForecastSource source)
		{ actor.OutputHandler.Send("There is no predictable weather here."); return; }
		var saved = actor.EffectsOfType<WeatherForecastReading>().Where(x => x.ControllerId == controller.Id).OrderByDescending(x => x.Day).ToArray();
		var reading = saved.FirstOrDefault(x => x.Day == source.ForecastDay);
		if (reading is null && !CanObserve(actor))
		{
			reading = saved.FirstOrDefault(x => source.ForecastDay >= x.Day && source.ForecastDay - x.Day <= x.Horizon);
			if (reading is null) { actor.OutputHandler.Send("You need a view of the outdoor conditions to make a weather forecast."); return; }
		}
		if (reading is null)
		{
			if (!actor.Gameworld.GetCheck(CheckType.WeatherForecastCapability).Check(actor, Difficulty.Automatic).IsPass())
			{ actor.OutputHandler.Send("You lack the knowledge to read the coming weather."); return; }
			IReadOnlyList<WeatherForecastPoint> points;
			try { points = source.GetForecast(); }
			catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or OverflowException)
			{ actor.OutputHandler.Send("You cannot make a useful forecast here at present."); return; }
			var results = actor.Gameworld.GetCheck(CheckType.WeatherForecast).CheckAgainstAllDifficulties(actor, Difficulty.Normal, null);
			var outcome = results.GetValueOrDefault(Difficulty.Normal)?.Outcome ?? Outcome.NotTested;
			var seed = (ulong)Random.Shared.NextInt64();
			reading = new(actor, controller.Id, source.ForecastDay, source.ForecastHorizonDays, outcome,
				Interpret(controller, points, results.ToDictionary(x => x.Key, x => x.Value.Outcome), new WeatherRandom(seed)));
			foreach (var old in saved) actor.RemoveEffect(old);
			actor.AddEffect(reading);
		}
		var age = source.ForecastDay - reading.Day;
		var header = $"Weather outlook — {(age == 0 ? "made today" : $"made {age.ToStringN0(actor)} game days ago")}";
		var sb = new StringBuilder(header.GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine();
		if (table)
		{
			sb.AppendLine(StringUtilities.GetTextTable(reading.Periods.Select(x => new[]
			{
				PeriodLabel(x, age, actor), x.Conditions, x.Wind, Temperature(actor, x), x.Hazards
			}), new[] { "Period", "Expected Conditions", "Wind", "Approx. Temperature", "Hazards" }, actor));
		}
		else
		{
			foreach (var period in reading.Periods)
				sb.AppendLine($"{PeriodLabel(period, age, actor).ColourName()}: You expect {period.Conditions}, with {period.Wind.ToLowerInvariant()} and temperatures around {Temperature(actor, period)}.{(period.Hazards.Length > 0 ? $" There may be {period.Hazards.ToLowerInvariant()}." : "")}");
		}
		sb.AppendLine("The more distant outlook is less certain.");
		actor.OutputHandler.Send(table ? sb.ToString() : sb.ToString().Wrap(actor.InnerLineFormatLength));
	}

	private static string Temperature(ICharacter actor, WeatherOutlookPeriod period)
	{
		var format = (NumberFormatInfo)NumberFormatInfo.GetInstance(actor).Clone();
		format.NumberDecimalDigits = 0;
		var system = actor.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference;
		return $"{actor.Gameworld.UnitManager.DescribeDecimal(period.MinimumTemperature, UnitType.Temperature, system, format)} to {actor.Gameworld.UnitManager.DescribeDecimal(period.MaximumTemperature, UnitType.Temperature, system, format)}";
	}

	internal static string PeriodLabel(WeatherOutlookPeriod period, long age, IFormatProvider viewer)
	{
		var day = period.DayOffset - age;
		var label = day switch { 0 => "Today", 1 => "Tomorrow", -1 => "Yesterday", < 0 => $"{(-day).ToString("N0", viewer)} days ago", _ => $"In {day.ToString("N0", viewer)} days" };
		return period.DayPart is { Length: > 0 } part ? $"{label}, {part}" : label;
	}

	private static string WindPhrase(WindLevel wind) => wind switch
	{
		WindLevel.None or WindLevel.Still => "calm air",
		WindLevel.OccasionalBreeze => "occasional breezes", WindLevel.Breeze => "a steady breeze",
		WindLevel.Wind => "moderate winds", WindLevel.StrongWind => "strong winds",
		WindLevel.GaleWind => "gale-force winds", WindLevel.HurricaneWind => "hurricane-force winds",
		WindLevel.MaelstromWind => "violent, swirling winds", _ => wind.Describe()
	};

	internal static IReadOnlyList<WeatherOutlookPeriod> Interpret(IWeatherController controller, IReadOnlyList<WeatherForecastPoint> points,
		IReadOnlyDictionary<Difficulty, Outcome> results, Random random)
	{
		var dayMinutes = controller.FeedClock.HoursPerDay * controller.FeedClock.MinutesPerHour;
		var local = controller.FeedClock.CurrentTime.GetTimeByTimezone(controller.FeedClockTimeZone);
		var localMinute = local.Hours * controller.FeedClock.MinutesPerHour + local.Minutes;
		var start = points[0].Minute;
		var interpreted = new List<(int Day, TimeOfDay Time, IWeatherEvent Weather, double Temperature)>();
		foreach (var point in points)
		{
			var day = (int)((point.Minute - start + localMinute) / dayMinutes);
			var difficulty = (Difficulty)Math.Min((int)Difficulty.ExtremelyHard, (int)Difficulty.Normal + (day + 1) / 2);
			var outcome = results.GetValueOrDefault(difficulty, Outcome.MajorFail);
			var error = Math.Clamp(((int)Outcome.MajorPass - (int)outcome) * 0.12 + day * 0.025, 0.0, 0.9);
			var season = controller.Gameworld.Seasons.Get(point.SeasonId);
			var weather = controller.Gameworld.WeatherEvents.Get(point.EventId);
			var shift = 0.0;
			if (random.NextDouble() < error)
			{
				// A fresh climate-compatible alternative, never an impossible event from another climate.
				weather = controller.RegionalClimate.ClimateModel.HandleWeatherTick(null, season, point.TimeOfDay, 0, random.NextDouble) ?? weather;
				shift = (random.NextDouble() - 0.5) * 10 * error;
			}
			var temperature = controller.RegionalClimate.HourlyBaseTemperaturesBySeason[(season, point.LocalHour)] + point.TemperatureFluctuation + weather.TemperatureEffect + shift;
			interpreted.Add((day, point.TimeOfDay, weather, temperature));
		}
		return interpreted.GroupBy(x => (x.Day, Time: x.Day == 0 ? x.Time : (TimeOfDay?)null)).Select(group =>
		{
			var weather = group.GroupBy(x => x.Weather).OrderByDescending(x => x.Count()).First().Key;
			var period = group.Key.Day switch { 0 => $"Today, {group.Key.Time!.Value.DescribeEnum().ToLowerInvariant()}", 1 => "Tomorrow", _ => $"In {group.Key.Day} days" };
			var hazards = new List<string>();
			if (group.Any(x => x.Weather.Hazards is { LightningChance: > 0 } or { AtmosphericLightningChance: > 0 })) hazards.Add("lightning");
			if (group.Any(x => x.Weather.Hazards is { AtmosphereGasId: > 0 })) hazards.Add("hazardous air");
			var description = string.IsNullOrWhiteSpace(weather.Hazards?.ForecastDescription) ? weather.Precipitation switch
			{
				PrecipitationLevel.Parched => "very dry conditions", PrecipitationLevel.Dry => "dry conditions",
				PrecipitationLevel.Humid => "humid conditions", _ => weather.Precipitation.Describe().ToLowerInvariant()
			} : weather.Hazards.ForecastDescription;
			return new WeatherOutlookPeriod(period, description, WindPhrase(weather.Wind), string.Join(" and ", hazards),
				Math.Floor(group.Min(x => x.Temperature) / 2) * 2, Math.Ceiling(group.Max(x => x.Temperature) / 2) * 2,
				group.Key.Day, group.Key.Time?.DescribeEnum().ToLowerInvariant());
		}).ToArray();
	}
}
