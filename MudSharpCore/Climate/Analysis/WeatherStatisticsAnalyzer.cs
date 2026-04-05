using Microsoft.EntityFrameworkCore;
using MudSharp.Celestial;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Calendar = MudSharp.TimeAndDate.Date.Calendar;
using Clock = MudSharp.TimeAndDate.Time.Clock;

namespace MudSharp.Climate.Analysis;

internal sealed class WeatherStatisticsRequest
{
    public required IWeatherController Controller { get; init; }
    public int Years { get; init; } = 20;
    public int BurnInYears { get; init; } = 2;
    public int? Seed { get; init; }
}

internal sealed class WeatherStatisticsSimulationRequest
{
    public required int Years { get; init; }
    public required int BurnInYears { get; init; }
    public int? Seed { get; init; }
    public required IReadOnlyDictionary<long, ISeason> SeasonInfo { get; init; }
    public required WeatherTransitionSnapshot TransitionSnapshot { get; init; }
    public required WeatherSimulationContext SimulationContext { get; init; }
    public required long InitialSeasonId { get; init; }
    public long? InitialWeatherEventId { get; init; }
    public int InitialConsecutiveUnchangedPeriods { get; init; }
    public int InitialMinuteCounter { get; init; }
}

internal sealed class WeatherStatisticsResult
{
    public required long ControllerId { get; init; }
    public required string ControllerName { get; init; }
    public required int Years { get; init; }
    public required int BurnInYears { get; init; }
    public required long SimulatedMinutes { get; init; }
    public required IReadOnlyList<WeatherStatisticsCsvRow> Rows { get; init; }
}

internal sealed class WeatherStatisticsCsvRow
{
    public required string Season { get; init; }
    public required string MetricType { get; init; }
    public required string Key { get; init; }
    public int? Hour { get; init; }
    public required string Statistic { get; init; }
    public required double Value { get; init; }
    public required string Unit { get; init; }
    public required long SampleCount { get; init; }
    public required long SeasonMinutes { get; init; }
}

internal sealed class WeatherEventInfo
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required PrecipitationLevel Precipitation { get; init; }
    public required WindLevel Wind { get; init; }
    public required double TemperatureEffect { get; init; }
    public double PrecipitationTemperatureEffect { get; init; }
    public double WindTemperatureEffect { get; init; }
    public required HashSet<TimeOfDay> PermittedTimesOfDay { get; init; }
}

internal sealed class WeatherTransitionSnapshot
{
    private readonly IReadOnlyDictionary<long, WeatherEventInfo> _events;
    private readonly Dictionary<(long SeasonId, long EventId), double> _baseChangeChance = new();
    private readonly Dictionary<(long SeasonId, long EventId), List<(long EventId, double Chance)>> _transitions = new();
    private readonly Dictionary<(long SeasonId, long EventId, TimeOfDay Time), (long EventId, double Chance)[]> _cachedTransitions = new();
    private readonly Dictionary<(long SeasonId, long EventId, TimeOfDay Time), double> _cachedTransitionTotals = new();
    private readonly Dictionary<(long SeasonId, TimeOfDay Time), (long EventId, double Chance)[]> _seasonEventCache = new();
    private readonly Dictionary<(long SeasonId, TimeOfDay Time), double> _seasonEventTotals = new();
    private readonly Dictionary<long, double> _maximumAdditionalChangeChanceFromStableWeather = new();
    private readonly Dictionary<long, double> _incrementalAdditionalChangeChanceFromStableWeather = new();
    private static readonly TimeOfDay[] _timesOfDay = Enum.GetValues<TimeOfDay>();

    public WeatherTransitionSnapshot(
        int minuteProcessingInterval,
        IReadOnlyDictionary<long, WeatherEventInfo> events,
        IReadOnlyDictionary<long, double> maximumAdditionalChangeChanceFromStableWeather,
        IReadOnlyDictionary<long, double> incrementalAdditionalChangeChanceFromStableWeather,
        IReadOnlyDictionary<(long SeasonId, long EventId), double> baseChangeChance,
        IReadOnlyDictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>> transitions)
    {
        MinuteProcessingInterval = minuteProcessingInterval;
        _events = events;

        foreach (KeyValuePair<long, double> item in maximumAdditionalChangeChanceFromStableWeather)
        {
            _maximumAdditionalChangeChanceFromStableWeather[item.Key] = item.Value;
        }

        foreach (KeyValuePair<long, double> item in incrementalAdditionalChangeChanceFromStableWeather)
        {
            _incrementalAdditionalChangeChanceFromStableWeather[item.Key] = item.Value;
        }

        foreach (KeyValuePair<(long SeasonId, long EventId), double> item in baseChangeChance)
        {
            _baseChangeChance[item.Key] = item.Value;
        }

        foreach (KeyValuePair<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>> item in transitions)
        {
            _transitions[item.Key] = item.Value.ToList();
        }

        RecalculateCaches();
    }

    public int MinuteProcessingInterval { get; }

    public WeatherEventInfo? GetEvent(long? eventId)
    {
        if (eventId is null)
        {
            return null;
        }

        return _events.GetValueOrDefault(eventId.Value);
    }

    public IEnumerable<long> SeasonEventIds(long seasonId)
    {
        HashSet<long> ids = new();
        foreach ((long SeasonId, long EventId) key in _baseChangeChance.Keys.Where(x => x.SeasonId == seasonId))
        {
            ids.Add(key.EventId);
        }

        foreach (KeyValuePair<(long SeasonId, long EventId), List<(long EventId, double Chance)>> kvp in _transitions.Where(x => x.Key.SeasonId == seasonId))
        {
            ids.Add(kvp.Key.EventId);
            foreach ((long EventId, double Chance) transition in kvp.Value)
            {
                ids.Add(transition.EventId);
            }
        }

        return ids;
    }

    public long? NextWeatherEvent(
        long? currentWeatherEventId,
        long currentSeasonId,
        TimeOfDay currentTime,
        int consecutiveUnchangedPeriods,
        Random random)
    {
        if (currentWeatherEventId is null)
        {
            return RandomEventForSeason(currentSeasonId, currentTime, random);
        }

        bool hasData = _baseChangeChance.ContainsKey((currentSeasonId, currentWeatherEventId.Value)) &&
                      _transitions.ContainsKey((currentSeasonId, currentWeatherEventId.Value));

        WeatherEventInfo currentEvent = GetEvent(currentWeatherEventId.Value);
        bool forceChange = !hasData || currentEvent is null || !currentEvent.PermittedTimesOfDay.Contains(currentTime);

        double baseChance = _baseChangeChance.GetValueOrDefault((currentSeasonId, currentWeatherEventId.Value));
        double maxAdditional = _maximumAdditionalChangeChanceFromStableWeather.GetValueOrDefault(currentSeasonId);
        double incrementalAdditional = _incrementalAdditionalChangeChanceFromStableWeather.GetValueOrDefault(currentSeasonId);
        bool shouldChange = forceChange ||
                      (random.NextDouble() <= (baseChance + Math.Min(maxAdditional,
                          incrementalAdditional * consecutiveUnchangedPeriods)));

        if (!shouldChange)
        {
            return currentWeatherEventId;
        }

        if (_cachedTransitions.TryGetValue((currentSeasonId, currentWeatherEventId.Value, currentTime), out (long EventId, double Chance)[] options) &&
            options.Length > 0)
        {
            return SelectWeighted(options, _cachedTransitionTotals[(currentSeasonId, currentWeatherEventId.Value, currentTime)], random);
        }

        if (forceChange)
        {
            return RandomEventForSeason(currentSeasonId, currentTime, random) ?? currentWeatherEventId;
        }

        return currentWeatherEventId;
    }

    private long? RandomEventForSeason(long seasonId, TimeOfDay time, Random random)
    {
        if (!_seasonEventCache.TryGetValue((seasonId, time), out (long EventId, double Chance)[] options))
        {
            return null;
        }

        return SelectWeighted(options, _seasonEventTotals[(seasonId, time)], random);
    }

    private static long? SelectWeighted((long EventId, double Chance)[] options, double total, Random random)
    {
        if (options.Length == 0)
        {
            return null;
        }

        if (total <= 0.0)
        {
            return options[random.Next(options.Length)].EventId;
        }

        double roll = random.NextDouble() * total;
        foreach ((long EventId, double Chance) option in options)
        {
            if (option.Chance <= 0.0)
            {
                continue;
            }

            if ((roll -= option.Chance) <= 0.0)
            {
                return option.EventId;
            }
        }

        return options[^1].EventId;
    }

    private void RecalculateCaches()
    {
        _cachedTransitions.Clear();
        _cachedTransitionTotals.Clear();
        foreach (KeyValuePair<(long SeasonId, long EventId), List<(long EventId, double Chance)>> kvp in _transitions)
        {
            foreach (TimeOfDay time in _timesOfDay)
            {
                (long EventId, double Chance)[] values = kvp.Value
                    .Where(x => _events.TryGetValue(x.EventId, out WeatherEventInfo weatherEvent) && weatherEvent.PermittedTimesOfDay.Contains(time))
                    .ToArray();
                _cachedTransitions[(kvp.Key.SeasonId, kvp.Key.EventId, time)] = values;
                _cachedTransitionTotals[(kvp.Key.SeasonId, kvp.Key.EventId, time)] = values.Sum(x => x.Chance);
            }
        }

        _seasonEventCache.Clear();
        _seasonEventTotals.Clear();
        Dictionary<(long SeasonId, TimeOfDay Time), List<(long EventId, double Chance)>> temp = new();
        foreach (KeyValuePair<(long SeasonId, long EventId), double> kvp in _baseChangeChance)
        {
            if (!_events.TryGetValue(kvp.Key.EventId, out WeatherEventInfo weatherEvent))
            {
                continue;
            }

            foreach (TimeOfDay time in weatherEvent.PermittedTimesOfDay)
            {
                (long SeasonId, TimeOfDay time) key = (kvp.Key.SeasonId, time);
                if (!temp.TryGetValue(key, out List<(long EventId, double Chance)> list))
                {
                    list = new List<(long EventId, double Chance)>();
                    temp[key] = list;
                }

                list.Add((kvp.Key.EventId, kvp.Value));
            }
        }

        foreach (KeyValuePair<(long SeasonId, TimeOfDay Time), List<(long EventId, double Chance)>> kvp in temp)
        {
            (long EventId, double Chance)[] arr = kvp.Value.ToArray();
            _seasonEventCache[kvp.Key] = arr;
            _seasonEventTotals[kvp.Key] = arr.Sum(x => x.Chance);
        }
    }
}

internal static class WeatherStatisticsMath
{
    public static double Percentile(IReadOnlyList<double> values, double percentile)
    {
        if (values.Count == 0)
        {
            return 0.0;
        }

        if (values.Count == 1)
        {
            return values[0];
        }

        double clamped = Math.Max(0.0, Math.Min(1.0, percentile));
        List<double> sorted = values.OrderBy(x => x).ToList();
        double position = (sorted.Count - 1) * clamped;
        int lower = (int)Math.Floor(position);
        int upper = (int)Math.Ceiling(position);
        if (lower == upper)
        {
            return sorted[lower];
        }

        double fraction = position - lower;
        return sorted[lower] + ((sorted[upper] - sorted[lower]) * fraction);
    }
}

internal sealed class WeatherSimulationContext : IDisposable
{
    public required GeographicCoordinate Geography { get; init; }
    public required CircularRange<ISeason> SeasonRotation { get; init; }
    public required int FeedHoursPerDay { get; init; }
    public required int FeedMinutesPerHour { get; init; }
    public required double YearLengthDays { get; init; }
    public required Func<double> GetCelestialDay { get; init; }
    public required Func<TimeOfDay> GetCurrentTimeOfDay { get; init; }
    public required Func<int> GetFeedLocalHour { get; init; }
    public required Action AdvanceOneFeedMinute { get; init; }
    public required Func<bool> ConsumeYearBoundary { get; init; }
    public required Func<long, int, double> GetBaseTemperature { get; init; }
    public double InitialTemperatureFluctuation { get; init; }
    public Func<double, int, double>? AdvanceTemperatureFluctuation { get; init; }
    public Action? DisposeAction { private get; init; }
    private bool _disposed;
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        DisposeAction?.Invoke();
    }
}

internal sealed class SeasonAccumulator
{
    public long TotalMinutes { get; private set; }
    public Dictionary<long, long> EventMinutes { get; } = new();
    public Dictionary<PrecipitationLevel, long> PrecipitationMinutes { get; } = new();
    public Dictionary<WindLevel, long> WindMinutes { get; } = new();
    public Dictionary<int, HourlySeasonStatistics> HourlyStatistics { get; } = new();
    public long RainMinutes { get; private set; }
    public long SnowMinutes { get; private set; }
    public long WetMinutes { get; private set; }

    public void RecordOccupancy(WeatherEventInfo? weatherEvent)
    {
        TotalMinutes++;
        if (weatherEvent is null)
        {
            return;
        }

        EventMinutes[weatherEvent.Id] = EventMinutes.GetValueOrDefault(weatherEvent.Id) + 1;
        PrecipitationMinutes[weatherEvent.Precipitation] = PrecipitationMinutes.GetValueOrDefault(weatherEvent.Precipitation) + 1;
        WindMinutes[weatherEvent.Wind] = WindMinutes.GetValueOrDefault(weatherEvent.Wind) + 1;

        if (weatherEvent.Precipitation.IsRaining())
        {
            RainMinutes++;
        }

        if (weatherEvent.Precipitation.IsSnowing())
        {
            SnowMinutes++;
        }

        if (weatherEvent.Precipitation.IsRaining() || weatherEvent.Precipitation.IsSnowing())
        {
            WetMinutes++;
        }
    }
}

internal sealed class HourlySeasonStatistics
{
    public HourlyTemperatureStatistics Shelter { get; } = new();
    public HourlyTemperatureStatistics Outdoors { get; } = new();
}

internal sealed class HourlyTemperatureStatistics
{
    public List<double> HourlyMeans { get; } = new();
    public List<double> HourlyMins { get; } = new();
    public List<double> HourlyMaxs { get; } = new();
}

internal sealed class HourlyBlockTracker
{
    private bool _hasBlock;
    private long _seasonId;
    private int _hour;
    private double _shelterSum;
    private double _outdoorsSum;
    private long _count;
    private double _shelterMin;
    private double _shelterMax;
    private double _outdoorsMin;
    private double _outdoorsMax;

    public void Reset()
    {
        _hasBlock = false;
        _seasonId = 0;
        _hour = 0;
        _shelterSum = 0.0;
        _outdoorsSum = 0.0;
        _count = 0;
        _shelterMin = 0.0;
        _shelterMax = 0.0;
        _outdoorsMin = 0.0;
        _outdoorsMax = 0.0;
    }

    public void RecordMinute(
        long seasonId,
        int hour,
        double shelteredTemperature,
        double outdoorsTemperature,
        IReadOnlyDictionary<long, SeasonAccumulator> accumulators)
    {
        if (!_hasBlock)
        {
            StartNewBlock(seasonId, hour, shelteredTemperature, outdoorsTemperature);
            return;
        }

        if (_seasonId == seasonId && _hour == hour)
        {
            _shelterSum += shelteredTemperature;
            _outdoorsSum += outdoorsTemperature;
            _count++;
            if (shelteredTemperature < _shelterMin)
            {
                _shelterMin = shelteredTemperature;
            }

            if (shelteredTemperature > _shelterMax)
            {
                _shelterMax = shelteredTemperature;
            }

            if (outdoorsTemperature < _outdoorsMin)
            {
                _outdoorsMin = outdoorsTemperature;
            }

            if (outdoorsTemperature > _outdoorsMax)
            {
                _outdoorsMax = outdoorsTemperature;
            }

            return;
        }

        FlushCurrentBlock(accumulators);
        StartNewBlock(seasonId, hour, shelteredTemperature, outdoorsTemperature);
    }

    public void Finalize(IReadOnlyDictionary<long, SeasonAccumulator> accumulators)
    {
        FlushCurrentBlock(accumulators);
    }

    private void StartNewBlock(long seasonId, int hour, double shelteredTemperature, double outdoorsTemperature)
    {
        _hasBlock = true;
        _seasonId = seasonId;
        _hour = hour;
        _shelterSum = shelteredTemperature;
        _outdoorsSum = outdoorsTemperature;
        _count = 1;
        _shelterMin = shelteredTemperature;
        _shelterMax = shelteredTemperature;
        _outdoorsMin = outdoorsTemperature;
        _outdoorsMax = outdoorsTemperature;
    }

    private void FlushCurrentBlock(IReadOnlyDictionary<long, SeasonAccumulator> accumulators)
    {
        if (!_hasBlock || _count <= 0)
        {
            _hasBlock = false;
            return;
        }

        if (accumulators.TryGetValue(_seasonId, out SeasonAccumulator accumulator))
        {
            if (!accumulator.HourlyStatistics.TryGetValue(_hour, out HourlySeasonStatistics hourly))
            {
                hourly = new HourlySeasonStatistics();
                accumulator.HourlyStatistics[_hour] = hourly;
            }

            hourly.Shelter.HourlyMins.Add(_shelterMin);
            hourly.Shelter.HourlyMaxs.Add(_shelterMax);
            hourly.Shelter.HourlyMeans.Add(_shelterSum / _count);

            hourly.Outdoors.HourlyMins.Add(_outdoorsMin);
            hourly.Outdoors.HourlyMaxs.Add(_outdoorsMax);
            hourly.Outdoors.HourlyMeans.Add(_outdoorsSum / _count);
        }

        _hasBlock = false;
    }
}

internal sealed class WeatherStatisticsAnalyzer
{
    public WeatherStatisticsResult Analyze(WeatherStatisticsRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Controller.RegionalClimate is null)
        {
            throw new ApplicationException("The selected weather controller has no regional climate.");
        }

        if (request.Controller.RegionalClimate.ClimateModel is null)
        {
            throw new ApplicationException("The selected weather controller's regional climate has no climate model.");
        }

        WeatherTransitionSnapshot transitionSnapshot = LoadTransitionSnapshot(request.Controller);
        WeatherSimulationContext simulationContext = BuildSimulationContext(request.Controller);
        Dictionary<long, ISeason> seasonInfo = request.Controller.RegionalClimate.Seasons
            .Distinct()
            .ToDictionary(x => x.Id);
        if (seasonInfo.Count == 0)
        {
            throw new ApplicationException("The selected weather controller has no seasons in its regional climate.");
        }

        long initialSeasonId = request.Controller.CurrentSeason?.Id ?? seasonInfo.Values.First().Id;
        if (!seasonInfo.ContainsKey(initialSeasonId))
        {
            initialSeasonId = seasonInfo.Values.First().Id;
        }

        WeatherStatisticsSimulationRequest simulationRequest = new()
        {
            Years = request.Years,
            BurnInYears = request.BurnInYears,
            Seed = request.Seed,
            SeasonInfo = seasonInfo,
            TransitionSnapshot = transitionSnapshot,
            SimulationContext = simulationContext,
            InitialSeasonId = initialSeasonId,
            InitialWeatherEventId = request.Controller.CurrentWeatherEvent?.Id,
            InitialConsecutiveUnchangedPeriods = request.Controller.ConsecutiveUnchangedPeriods,
            InitialMinuteCounter = request.Controller.MinuteCounter
        };

        return AnalyzeSimulation(simulationRequest, request.Controller.Id, request.Controller.Name);
    }

    internal WeatherStatisticsResult AnalyzeSimulation(
        WeatherStatisticsSimulationRequest request,
        long controllerId = 0,
        string controllerName = "Simulation")
    {
        if (request.Years <= 0)
        {
            throw new ArgumentException("Years must be greater than zero.", nameof(request));
        }
        if (request.BurnInYears < 0)
        {
            throw new ArgumentException("Burn-in years must be zero or greater.", nameof(request));
        }
        if (request.SeasonInfo.Count == 0)
        {
            throw new ArgumentException("At least one season is required for analysis.", nameof(request));
        }
        try
        {
            Random random = request.Seed.HasValue ? new Random(request.Seed.Value) : new Random();
            Dictionary<long, SeasonAccumulator> seasonAccumulators = request.SeasonInfo.Values.ToDictionary(x => x.Id, _ => new SeasonAccumulator());
            HourlyBlockTracker hourlyTracker = new();
            WeatherSimulationState state = new()
            {
                CurrentSeasonId = request.SeasonInfo.ContainsKey(request.InitialSeasonId)
                    ? request.InitialSeasonId
                    : request.SeasonInfo.Values.First().Id,
                CurrentWeatherEventId = request.InitialWeatherEventId,
                ConsecutiveUnchangedPeriods = Math.Max(0, request.InitialConsecutiveUnchangedPeriods),
                MinuteCounter = Math.Max(0, request.InitialMinuteCounter),
                CurrentTemperatureFluctuation = request.SimulationContext.InitialTemperatureFluctuation
            };
            if (state.CurrentWeatherEventId is null)
            {
                ISeason initialSeason = request.SimulationContext.SeasonRotation.Get(request.SimulationContext.GetCelestialDay());
                state.CurrentSeasonId = initialSeason.Id;
                state.CurrentWeatherEventId = request.TransitionSnapshot.NextWeatherEvent(
                    null,
                    state.CurrentSeasonId,
                    request.SimulationContext.GetCurrentTimeOfDay(),
                    state.ConsecutiveUnchangedPeriods,
                    random);
            }
            long expectedMinutesPerYear = (long)Math.Max(1.0,
                request.SimulationContext.YearLengthDays * request.SimulationContext.FeedHoursPerDay * request.SimulationContext.FeedMinutesPerHour);
            long maxMinutes = Math.Max(2_000_000L, expectedMinutesPerYear * (request.Years + request.BurnInYears + 2L) * 2L);
            bool collectData = request.BurnInYears == 0;
            int burnInYears = 0;
            int measuredYears = 0;
            long simulatedMinutes = 0L;
            while (measuredYears < request.Years)
            {
                if (++simulatedMinutes > maxMinutes)
                {
                    throw new ApplicationException("Weather analysis exceeded the safety limit for simulation minutes. Check celestial/year configuration.");
                }
                request.SimulationContext.AdvanceOneFeedMinute();
                if (++state.MinuteCounter >= request.TransitionSnapshot.MinuteProcessingInterval)
                {
                    ISeason season = request.SimulationContext.SeasonRotation.Get(request.SimulationContext.GetCelestialDay());
                    state.CurrentSeasonId = season.Id;
                    state.MinuteCounter = 0;
                    if (request.SimulationContext.AdvanceTemperatureFluctuation is not null)
                    {
                        state.CurrentTemperatureFluctuation = request.SimulationContext.AdvanceTemperatureFluctuation(
                            state.CurrentTemperatureFluctuation,
                            request.TransitionSnapshot.MinuteProcessingInterval);
                    }
                    long? nextEvent = request.TransitionSnapshot.NextWeatherEvent(
                        state.CurrentWeatherEventId,
                        state.CurrentSeasonId,
                        request.SimulationContext.GetCurrentTimeOfDay(),
                        state.ConsecutiveUnchangedPeriods,
                        random);
                    if (nextEvent == state.CurrentWeatherEventId)
                    {
                        state.ConsecutiveUnchangedPeriods++;
                    }
                    else
                    {
                        state.CurrentWeatherEventId = nextEvent;
                        state.ConsecutiveUnchangedPeriods = 0;
                    }
                }
                int localHour = request.SimulationContext.GetFeedLocalHour();
                WeatherEventInfo weatherEvent = request.TransitionSnapshot.GetEvent(state.CurrentWeatherEventId);
                double shelteredTemperature = request.SimulationContext.GetBaseTemperature(state.CurrentSeasonId, localHour) +
                                         state.CurrentTemperatureFluctuation +
                                         (weatherEvent?.TemperatureEffect ?? 0.0);
                double outdoorsTemperature = shelteredTemperature +
                                         (weatherEvent?.PrecipitationTemperatureEffect ?? 0.0) +
                                         (weatherEvent?.WindTemperatureEffect ?? 0.0);
                if (collectData)
                {
                    if (!seasonAccumulators.TryGetValue(state.CurrentSeasonId, out SeasonAccumulator seasonAccumulator))
                    {
                        seasonAccumulator = new SeasonAccumulator();
                        seasonAccumulators[state.CurrentSeasonId] = seasonAccumulator;
                    }
                    seasonAccumulator.RecordOccupancy(weatherEvent);
                    hourlyTracker.RecordMinute(
                        state.CurrentSeasonId,
                        localHour,
                        shelteredTemperature,
                        outdoorsTemperature,
                        seasonAccumulators);
                }
                if (!request.SimulationContext.ConsumeYearBoundary())
                {
                    continue;
                }
                if (!collectData)
                {
                    burnInYears++;
                    if (burnInYears >= request.BurnInYears)
                    {
                        collectData = true;
                        hourlyTracker.Reset();
                    }
                    continue;
                }
                measuredYears++;
            }
            hourlyTracker.Finalize(seasonAccumulators);
            List<WeatherStatisticsCsvRow> rows = BuildRows(request.SeasonInfo, seasonAccumulators, request.TransitionSnapshot,
                request.SimulationContext.FeedHoursPerDay);
            return new WeatherStatisticsResult
            {
                ControllerId = controllerId,
                ControllerName = controllerName,
                Years = request.Years,
                BurnInYears = request.BurnInYears,
                SimulatedMinutes = simulatedMinutes,
                Rows = rows
            };
        }
        finally
        {
            request.SimulationContext.Dispose();
        }
    }

    private static List<WeatherStatisticsCsvRow> BuildRows(
        IReadOnlyDictionary<long, ISeason> seasonInfo,
        IReadOnlyDictionary<long, SeasonAccumulator> seasonAccumulators,
        WeatherTransitionSnapshot transitionSnapshot,
        int hoursPerDay)
    {
        List<WeatherStatisticsCsvRow> rows = new();
        foreach (ISeason season in seasonInfo.Values.OrderBy(x => x.CelestialDayOnset).ThenBy(x => x.Name))
        {
            if (!seasonAccumulators.TryGetValue(season.Id, out SeasonAccumulator stats))
            {
                continue;
            }

            long totalMinutes = stats.TotalMinutes;
            List<long> eventIds = transitionSnapshot.SeasonEventIds(season.Id)
                .Concat(stats.EventMinutes.Keys)
                .Distinct()
                .OrderBy(x => transitionSnapshot.GetEvent(x)?.Name ?? x.ToString(CultureInfo.InvariantCulture))
                .ToList();

            foreach (long eventId in eventIds)
            {
                string eventName = transitionSnapshot.GetEvent(eventId)?.Name ?? eventId.ToString(CultureInfo.InvariantCulture);
                rows.AddRange(BuildMinuteAndPercentRows(season.Name, "WeatherEvent", eventName,
                    stats.EventMinutes.GetValueOrDefault(eventId), totalMinutes));
            }

            foreach (PrecipitationLevel precipitation in Enum.GetValues<PrecipitationLevel>())
            {
                rows.AddRange(BuildMinuteAndPercentRows(season.Name, "Precipitation", precipitation.Describe(),
                    stats.PrecipitationMinutes.GetValueOrDefault(precipitation), totalMinutes));
            }

            foreach (WindLevel wind in Enum.GetValues<WindLevel>())
            {
                rows.AddRange(BuildMinuteAndPercentRows(season.Name, "Wind", wind.Describe(),
                    stats.WindMinutes.GetValueOrDefault(wind), totalMinutes));
            }

            rows.AddRange(BuildMinuteAndPercentRows(season.Name, "RainfallProxy", "Rain", stats.RainMinutes, totalMinutes));
            rows.AddRange(BuildMinuteAndPercentRows(season.Name, "RainfallProxy", "Snow", stats.SnowMinutes, totalMinutes));
            rows.AddRange(BuildMinuteAndPercentRows(season.Name, "RainfallProxy", "Wet", stats.WetMinutes, totalMinutes));

            for (int hour = 0; hour < Math.Max(1, hoursPerDay); hour++)
            {
                bool hasSamples = stats.HourlyStatistics.TryGetValue(hour, out HourlySeasonStatistics hourly);
                HourlyTemperatureStatistics shelterStats = hasSamples ? hourly!.Shelter : null;
                HourlyTemperatureStatistics outdoorsStats = hasSamples ? hourly!.Outdoors : null;

                rows.AddRange(BuildTemperatureRows(season.Name, "Shelter", hour, totalMinutes, shelterStats));
                rows.AddRange(BuildTemperatureRows(season.Name, "Outdoors", hour, totalMinutes, outdoorsStats));
            }
        }

        return rows;
    }

    private static IEnumerable<WeatherStatisticsCsvRow> BuildTemperatureRows(
        string seasonName,
        string key,
        int hour,
        long totalMinutes,
        HourlyTemperatureStatistics? hourly)
    {
        int sampleCount = hourly?.HourlyMeans.Count ?? 0;
        double mean = sampleCount > 0 ? hourly!.HourlyMeans.Average() : 0.0;
        double likelyMin = sampleCount > 0 ? WeatherStatisticsMath.Percentile(hourly!.HourlyMins, 0.025) : 0.0;
        double likelyMax = sampleCount > 0 ? WeatherStatisticsMath.Percentile(hourly!.HourlyMaxs, 0.975) : 0.0;

        yield return new WeatherStatisticsCsvRow
        {
            Season = seasonName,
            MetricType = "TemperatureHourly",
            Key = key,
            Hour = hour,
            Statistic = "Mean",
            Value = mean,
            Unit = "temperature",
            SampleCount = sampleCount,
            SeasonMinutes = totalMinutes
        };

        yield return new WeatherStatisticsCsvRow
        {
            Season = seasonName,
            MetricType = "TemperatureHourly",
            Key = key,
            Hour = hour,
            Statistic = "LikelyMin",
            Value = likelyMin,
            Unit = "temperature",
            SampleCount = sampleCount,
            SeasonMinutes = totalMinutes
        };

        yield return new WeatherStatisticsCsvRow
        {
            Season = seasonName,
            MetricType = "TemperatureHourly",
            Key = key,
            Hour = hour,
            Statistic = "LikelyMax",
            Value = likelyMax,
            Unit = "temperature",
            SampleCount = sampleCount,
            SeasonMinutes = totalMinutes
        };
    }

    private static IEnumerable<WeatherStatisticsCsvRow> BuildMinuteAndPercentRows(
        string seasonName,
        string metricType,
        string key,
        long minutes,
        long totalMinutes)
    {
        yield return new WeatherStatisticsCsvRow
        {
            Season = seasonName,
            MetricType = metricType,
            Key = key,
            Statistic = "Minutes",
            Value = minutes,
            Unit = "minutes",
            SampleCount = totalMinutes,
            SeasonMinutes = totalMinutes
        };

        yield return new WeatherStatisticsCsvRow
        {
            Season = seasonName,
            MetricType = metricType,
            Key = key,
            Statistic = "Percent",
            Value = totalMinutes > 0 ? (minutes / (double)totalMinutes) * 100.0 : 0.0,
            Unit = "percent",
            SampleCount = totalMinutes,
            SeasonMinutes = totalMinutes
        };
    }
    private static WeatherTransitionSnapshot LoadTransitionSnapshot(IWeatherController controller)
    {
        using (new FMDB())
        {
            Models.ClimateModel dbModel = FMDB.Context.ClimateModels
                .Include(x => x.ClimateModelSeasons)
                .ThenInclude(x => x.SeasonEvents)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == controller.RegionalClimate.ClimateModel.Id);
            if (dbModel is null)
            {
                throw new ApplicationException("Failed to load climate model from the database for weather analysis.");
            }

            if (!dbModel.Type.Equals("terrestrial", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ApplicationException($"Weather analysis currently supports terrestrial climate models only. This model is '{dbModel.Type}'.");
            }

            Dictionary<long, double> maxAdditional = new();
            Dictionary<long, double> incremental = new();
            Dictionary<(long SeasonId, long EventId), double> baseChangeChance = new();
            Dictionary<(long SeasonId, long EventId), IReadOnlyList<(long EventId, double Chance)>> transitions = new();
            HashSet<long> eventIds = new();

            foreach (Models.ClimateModelSeason season in dbModel.ClimateModelSeasons)
            {
                maxAdditional[season.SeasonId] = season.MaximumAdditionalChangeChanceFromStableWeather;
                incremental[season.SeasonId] = season.IncrementalAdditionalChangeChanceFromStableWeather;
                foreach (Models.ClimateModelSeasonEvent seasonEvent in season.SeasonEvents)
                {
                    baseChangeChance[(season.SeasonId, seasonEvent.WeatherEventId)] = seasonEvent.ChangeChance;
                    eventIds.Add(seasonEvent.WeatherEventId);
                    List<(long EventId, double Chance)> parsedTransitions = ParseTransitions(seasonEvent.Transitions)
                        .Where(x => x.EventId != seasonEvent.WeatherEventId)
                        .ToList();
                    foreach ((long EventId, double Chance) transition in parsedTransitions)
                    {
                        eventIds.Add(transition.EventId);
                    }

                    transitions[(season.SeasonId, seasonEvent.WeatherEventId)] = parsedTransitions;
                }
            }

            if (controller.CurrentWeatherEvent is not null)
            {
                eventIds.Add(controller.CurrentWeatherEvent.Id);
            }

            Dictionary<long, WeatherEventInfo> eventInfo = new();
            foreach (long eventId in eventIds)
            {
                IWeatherEvent weatherEvent = controller.Gameworld.WeatherEvents.Get(eventId);
                if (weatherEvent is null)
                {
                    continue;
                }

                eventInfo[eventId] = new WeatherEventInfo
                {
                    Id = weatherEvent.Id,
                    Name = weatherEvent.Name,
                    Precipitation = weatherEvent.Precipitation,
                    Wind = weatherEvent.Wind,
                    TemperatureEffect = weatherEvent.TemperatureEffect,
                    PrecipitationTemperatureEffect = weatherEvent.PrecipitationTemperatureEffect,
                    WindTemperatureEffect = weatherEvent.WindTemperatureEffect,
                    PermittedTimesOfDay = weatherEvent.PermittedTimesOfDay.ToHashSet()
                };
            }

            return new WeatherTransitionSnapshot(
                dbModel.MinuteProcessingInterval,
                eventInfo,
                maxAdditional,
                incremental,
                baseChangeChance,
                transitions);
        }
    }

    internal static List<(long EventId, double Chance)> ParseTransitions(string xml)
    {
        List<(long EventId, double Chance)> result = new();
        if (string.IsNullOrWhiteSpace(xml))
        {
            return result;
        }

        XElement root;
        try
        {
            root = XElement.Parse(xml);
        }
        catch
        {
            return result;
        }

        foreach (XElement element in root.Elements())
        {
            string idText = element.Attribute("id")?.Value;
            string chanceText = element.Attribute("chance")?.Value;
            if (!long.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id))
            {
                continue;
            }

            if (!double.TryParse(chanceText, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double chance))
            {
                continue;
            }

            result.Add((id, chance));
        }

        return result;
    }

    private static WeatherSimulationContext BuildSimulationContext(IWeatherController controller)
    {
        if (controller.RegionalClimate.SeasonRotation is null)
        {
            throw new ApplicationException("The selected weather controller has no valid season rotation.");
        }
        if (controller.FeedClock.PrimaryTimezone is null)
        {
            throw new ApplicationException("The selected weather controller's feed clock has no primary timezone.");
        }
        if (controller.FeedClockTimeZone is null)
        {
            throw new ApplicationException("The selected weather controller has no feed clock timezone configured.");
        }
        Dictionary<(long Id, int DailyHour), double> baseTemperatures = controller.RegionalClimate.HourlyBaseTemperaturesBySeason
            .ToDictionary(x => (x.Key.Season.Id, x.Key.DailyHour), x => x.Value);
        foreach (ISeason season in controller.RegionalClimate.Seasons)
        {
            for (int hour = 0; hour < controller.FeedClock.HoursPerDay; hour++)
            {
                if (baseTemperatures.ContainsKey((season.Id, hour)))
                {
                    continue;
                }
                throw new ApplicationException($"The regional climate is missing a base temperature value for season {season.Name} at hour {hour}.");
            }
        }
        FeedClockState feedClockState = new(
            controller.FeedClock.CurrentTime.Hours,
            controller.FeedClock.CurrentTime.Minutes,
            controller.FeedClock.HoursPerDay,
            controller.FeedClock.MinutesPerHour,
            controller.FeedClock.PrimaryTimezone.OffsetHours,
            controller.FeedClock.PrimaryTimezone.OffsetMinutes,
            controller.FeedClockTimeZone.OffsetHours,
            controller.FeedClockTimeZone.OffsetMinutes);
        double realSecondsPerFeedMinute = controller.FeedClock.SecondsPerMinute / Math.Max(0.000_001, controller.FeedClock.InGameSecondsPerRealSecond);
        double seasonYearLength = controller.RegionalClimate.SeasonRotation.Ceiling;
        if (seasonYearLength <= 0.0)
        {
            seasonYearLength = controller.Celestial?.CelestialDaysPerYear ?? 365.0;
        }
        List<Action> cleanupActions = new();
        IReadOnlyDictionary<long, Models.Celestial> celestialModels = LoadCelestialModels(controller.Celestial);
        ICelestialSimulation? celestialSimulation = null;
        if (controller.Celestial is not null)
        {
            (ICelestialSimulation simulation, List<Action> actions) = BuildCelestialSimulation(controller.Celestial, celestialModels);
            celestialSimulation = simulation;
            cleanupActions.AddRange(actions);
        }
        RuntimeSimulationState runtime = new(feedClockState, celestialSimulation, realSecondsPerFeedMinute,
            seasonYearLength, controller.GeographyForTimeOfDay);
        return new WeatherSimulationContext
        {
            Geography = controller.GeographyForTimeOfDay,
            SeasonRotation = controller.RegionalClimate.SeasonRotation,
            FeedHoursPerDay = controller.FeedClock.HoursPerDay,
            FeedMinutesPerHour = controller.FeedClock.MinutesPerHour,
            YearLengthDays = seasonYearLength,
            GetCelestialDay = () => WeatherClimateUtilities.ApplySeasonPhaseShift(
                runtime.GetCelestialDay(),
                seasonYearLength,
                controller.OppositeHemisphere),
            GetCurrentTimeOfDay = runtime.GetTimeOfDay,
            GetFeedLocalHour = runtime.GetLocalHour,
            AdvanceOneFeedMinute = runtime.AdvanceOneFeedMinute,
            ConsumeYearBoundary = runtime.ConsumeYearBoundary,
            GetBaseTemperature = (seasonId, hour) => baseTemperatures[(seasonId, hour)],
            InitialTemperatureFluctuation = controller.CurrentTemperatureFluctuation,
            AdvanceTemperatureFluctuation = (currentOffset, tickMinutes) => WeatherClimateUtilities.AdvanceTemperatureFluctuation(
                currentOffset,
                controller.RegionalClimate.TemperatureFluctuationStandardDeviation,
                controller.RegionalClimate.TemperatureFluctuationPeriod,
                tickMinutes),
            DisposeAction = () =>
            {
                for (int i = cleanupActions.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        cleanupActions[i]();
                    }
                    catch
                    {
                        // Cleanup is best-effort only for simulation clones.
                    }
                }
            }
        };
    }
    private static IReadOnlyDictionary<long, MudSharp.Models.Celestial> LoadCelestialModels(ICelestialObject? root)
    {
        if (root is null)
        {
            return new Dictionary<long, MudSharp.Models.Celestial>();
        }
        HashSet<long> ids = new();
        GatherCelestialIds(root, ids);
        if (ids.Count == 0)
        {
            return new Dictionary<long, MudSharp.Models.Celestial>();
        }
        using (new FMDB())
        {
            return FMDB.Context.Celestials
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToDictionary(x => x.Id);
        }
    }
    private static void GatherCelestialIds(ICelestialObject celestial, ISet<long> ids)
    {
        if (!ids.Add(celestial.Id))
        {
            return;
        }
        switch (celestial)
        {
            case SunFromPlanetaryMoon sunFromMoon:
                GatherCelestialIds(sunFromMoon.Moon, ids);
                GatherCelestialIds(sunFromMoon.Sun, ids);
                break;
            case PlanetFromMoon planetFromMoon:
                GatherCelestialIds(planetFromMoon.Moon, ids);
                if (planetFromMoon.Sun is not null)
                {
                    GatherCelestialIds(planetFromMoon.Sun, ids);
                }
                break;
        }
    }
    private static (ICelestialSimulation Simulation, List<Action> CleanupActions) BuildCelestialSimulation(
        ICelestialObject source,
        IReadOnlyDictionary<long, MudSharp.Models.Celestial> celestialModels)
    {
        switch (source)
        {
            case NewSun newSun:
                {
                    (NewSun clonedSun, Clock clonedClock, ICalendar _, List<Action> cleanupActions) = CloneNewSun(newSun, celestialModels);
                    return (new ClockDrivenCelestialSimulation(clonedSun, new[] { clonedClock }), cleanupActions);
                }
            case PlanetaryMoon moon:
                {
                    (PlanetaryMoon clonedMoon, Clock clonedClock, ICalendar _, List<Action> cleanupActions) = ClonePlanetaryMoon(moon);
                    return (new ClockDrivenCelestialSimulation(clonedMoon, new[] { clonedClock }), cleanupActions);
                }
            case SunFromPlanetaryMoon sunFromMoon:
                {
                    (PlanetaryMoon clonedMoon, Clock moonClock, ICalendar _, List<Action> moonCleanupActions) = ClonePlanetaryMoon(sunFromMoon.Moon);
                    (NewSun clonedSun, Clock sunClock, ICalendar _, List<Action> sunCleanupActions) = CloneNewSun(sunFromMoon.Sun, celestialModels);
                    SunFromPlanetaryMoon clonedObject = new(clonedMoon, clonedSun);
                    SetNoSave(clonedObject);
                    List<Action> cleanupActions = new();
                    cleanupActions.AddRange(moonCleanupActions);
                    cleanupActions.AddRange(sunCleanupActions);
                    return (new ClockDrivenCelestialSimulation(clonedObject, new[] { moonClock, sunClock }), cleanupActions);
                }
            case PlanetFromMoon planetFromMoon:
                {
                    (ICelestialSimulation daySource, List<Action> dayCleanupActions) = BuildCelestialSimulation(planetFromMoon.Moon, celestialModels);
                    (ICelestialSimulation Simulation, List<Action> CleanupActions) timeSimulation;
                    if (planetFromMoon.Sun is null)
                    {
                        timeSimulation = (new FixedNightCelestialSimulation(daySource.CurrentCelestialDay), new List<Action>());
                    }
                    else
                    {
                        timeSimulation = BuildCelestialSimulation(planetFromMoon.Sun, celestialModels);
                    }
                    List<Action> cleanupActions = new();
                    cleanupActions.AddRange(dayCleanupActions);
                    cleanupActions.AddRange(timeSimulation.CleanupActions);
                    return (new CompositeCelestialSimulation(daySource, timeSimulation.Simulation), cleanupActions);
                }
            default:
                throw new ApplicationException($"Weather analysis does not support celestial type {source.GetType().Name}.");
        }
    }
    private static (NewSun Sun, Clock Clock, ICalendar Calendar, List<Action> CleanupActions) CloneNewSun(
        NewSun source,
        IReadOnlyDictionary<long, MudSharp.Models.Celestial> celestialModels)
    {
        if (!celestialModels.TryGetValue(source.Id, out Models.Celestial dbCelestial))
        {
            throw new ApplicationException($"Could not load celestial definition for {source.Name} (#{source.Id}).");
        }
        Clock clonedClock = CloneClock(source.Clock);
        Calendar clonedCalendar = CloneCalendar(source.Calendar, clonedClock);
        NewSun clonedSun = new(XElement.Parse(dbCelestial.Definition), clonedClock, source.Gameworld);
        SetNoSave(clonedSun);
        clonedSun.Calendar = clonedCalendar;
        clonedSun.MeanAnomalyAngleAtEpoch = source.MeanAnomalyAngleAtEpoch;
        clonedSun.AnomalyChangeAnglePerDay = source.AnomalyChangeAnglePerDay;
        clonedSun.EclipticLongitude = source.EclipticLongitude;
        clonedSun.EquatorialObliquity = source.EquatorialObliquity;
        clonedSun.OrbitalEccentricity = source.OrbitalEccentricity;
        clonedSun.OrbitalSemiMajorAxis = source.OrbitalSemiMajorAxis;
        clonedSun.ApparentAngularRadius = source.ApparentAngularRadius;
        clonedSun.DayNumberAtEpoch = source.DayNumberAtEpoch;
        clonedSun.CurrentDayNumberOffset = source.CurrentDayNumberOffset;
        clonedSun.SiderealTimeAtEpoch = source.SiderealTimeAtEpoch;
        clonedSun.SiderealTimePerDay = source.SiderealTimePerDay;
        clonedSun.KepplerC1Approximant = source.KepplerC1Approximant;
        clonedSun.KepplerC2Approximant = source.KepplerC2Approximant;
        clonedSun.KepplerC3Approximant = source.KepplerC3Approximant;
        clonedSun.KepplerC4Approximant = source.KepplerC4Approximant;
        clonedSun.KepplerC5Approximant = source.KepplerC5Approximant;
        clonedSun.KepplerC6Approximant = source.KepplerC6Approximant;
        clonedSun.CelestialDaysPerYear = source.CelestialDaysPerYear;
        clonedSun.EpochDate = clonedCalendar.GetDate(source.EpochDate.GetDateString());
        clonedSun.Changed = false;
        return (clonedSun, clonedClock, clonedCalendar, new List<Action>
        {
            () => clonedClock.MinutesUpdated -= clonedSun.AddMinutes,
            () => DetachCalendarFromClock(clonedCalendar, clonedClock)
        });
    }
    private static (PlanetaryMoon Moon, Clock Clock, ICalendar Calendar, List<Action> CleanupActions) ClonePlanetaryMoon(PlanetaryMoon source)
    {
        Clock clonedClock = CloneClock(source.Clock);
        Calendar clonedCalendar = CloneCalendar(source.Calendar, clonedClock);
        PlanetaryMoon clonedMoon = new(clonedCalendar, clonedClock);
        SetNoSave(clonedMoon);
        clonedMoon.MeanAnomalyAngleAtEpoch = source.MeanAnomalyAngleAtEpoch;
        clonedMoon.AnomalyChangeAnglePerDay = source.AnomalyChangeAnglePerDay;
        clonedMoon.ArgumentOfPeriapsis = source.ArgumentOfPeriapsis;
        clonedMoon.LongitudeOfAscendingNode = source.LongitudeOfAscendingNode;
        clonedMoon.OrbitalInclination = source.OrbitalInclination;
        clonedMoon.OrbitalEccentricity = source.OrbitalEccentricity;
        clonedMoon.OrbitalSemiMajorAxis = source.OrbitalSemiMajorAxis;
        clonedMoon.DayNumberAtEpoch = source.DayNumberAtEpoch;
        clonedMoon.SiderealTimeAtEpoch = source.SiderealTimeAtEpoch;
        clonedMoon.SiderealTimePerDay = source.SiderealTimePerDay;
        clonedMoon.CelestialDaysPerYear = source.CelestialDaysPerYear;
        clonedMoon.PeakIllumination = source.PeakIllumination;
        clonedMoon.FullMoonReferenceDay = source.FullMoonReferenceDay;
        clonedMoon.EpochDate = clonedCalendar.GetDate(source.EpochDate.GetDateString());
        clonedMoon.Changed = false;
        return (clonedMoon, clonedClock, clonedCalendar, new List<Action>
        {
            () => clonedClock.MinutesUpdated -= clonedMoon.AddMinutes,
            () => DetachCalendarFromClock(clonedCalendar, clonedClock)
        });
    }
    private static Clock CloneClock(IClock sourceClock)
    {
        if (sourceClock is not Clock concreteClock)
        {
            throw new ApplicationException($"Weather analysis requires concrete clock implementations. Clock {sourceClock.Name} is not supported.");
        }
        IMudTimeZone primary = concreteClock.PrimaryTimezone;
        if (primary is null)
        {
            throw new ApplicationException($"The clock {sourceClock.Name} has no primary timezone.");
        }
        MudTimeZone clonedPrimary = new(0, primary.OffsetHours, primary.OffsetMinutes,
            primary.Description, primary.Alias);
        SetNoSave(clonedPrimary);
        Clock clonedClock = new(concreteClock.SaveToXml(), concreteClock.Gameworld, clonedPrimary,
            concreteClock.CurrentTime.Hours, concreteClock.CurrentTime.Minutes, concreteClock.CurrentTime.Seconds);
        SetNoSave(clonedClock);
        clonedClock.AddTimezone(clonedPrimary);
        foreach (IMudTimeZone timezone in concreteClock.Timezones.Where(x => x.Id != primary.Id))
        {
            MudTimeZone clonedTimezone = new(0, timezone.OffsetHours,
                timezone.OffsetMinutes, timezone.Description, timezone.Alias);
            SetNoSave(clonedTimezone);
            clonedClock.AddTimezone(clonedTimezone);
        }
        clonedClock.Changed = false;
        return clonedClock;
    }
    private static Calendar CloneCalendar(ICalendar sourceCalendar, IClock feedClock)
    {
        if (sourceCalendar is not Calendar concreteCalendar)
        {
            throw new ApplicationException($"Weather analysis requires concrete calendar implementations. Calendar {sourceCalendar.Name} is not supported.");
        }
        Calendar clonedCalendar = new(concreteCalendar.SaveToXml(), concreteCalendar.Gameworld);
        SetNoSave(clonedCalendar);
        clonedCalendar.SetDate(concreteCalendar.CurrentDate.GetDateString());
        clonedCalendar.FeedClock = feedClock;
        clonedCalendar.Changed = false;
        return clonedCalendar;
    }
    private static void SetNoSave(object? item)
    {
        switch (item)
        {
            case null:
                return;
            case SaveableItem saveableItem:
                saveableItem.SetNoSave(true);
                saveableItem.Changed = false;
                return;
            case SavableKeywordedItem savableKeywordedItem:
                savableKeywordedItem.SetNoSave(true);
                savableKeywordedItem.Changed = false;
                return;
            case LateInitialisingItem lateInitialisingItem:
                lateInitialisingItem.SetNoSave(true);
                lateInitialisingItem.Changed = false;
                return;
            case LateKeywordedInitialisingItem lateKeywordedInitialisingItem:
                lateKeywordedInitialisingItem.SetNoSave(true);
                lateKeywordedInitialisingItem.Changed = false;
                return;
        }
    }
    private static void DetachCalendarFromClock(ICalendar calendar, IClock clock)
    {
        if (calendar is Calendar concreteCalendar && concreteCalendar.CurrentDate is not null)
        {
            clock.DaysUpdated -= concreteCalendar.CurrentDate.AdvanceDays;
        }
    }
    private static Action CreateClockMinutesCleanup(IClock clock, object target, string methodName)
    {
        try
        {
            MethodInfo method = target.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method is null)
            {
                return () => { };
            }
            ClockEventHandler handler = (ClockEventHandler)Delegate.CreateDelegate(typeof(ClockEventHandler), target, method);
            return () => clock.MinutesUpdated -= handler;
        }
        catch
        {
            return () => { };
        }
    }
    private static void SetProtectedProperty(object target, string propertyName, object value)
    {
        PropertyInfo prop = target.GetType().GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (prop is null || prop.SetMethod is null)
        {
            throw new ApplicationException($"Could not set protected property {propertyName} on {target.GetType().Name}.");
        }
        prop.SetValue(target, value);
    }

    private static TimeOfDay DetermineTimeOfDayByHour(int localHour, int hoursPerDay)
    {
        if (hoursPerDay <= 0)
        {
            return TimeOfDay.Night;
        }

        double fraction = localHour / (double)hoursPerDay;
        if (fraction < 0.20)
        {
            return TimeOfDay.Night;
        }

        if (fraction < 0.30)
        {
            return TimeOfDay.Dawn;
        }

        if (fraction < 0.55)
        {
            return TimeOfDay.Morning;
        }

        if (fraction < 0.80)
        {
            return TimeOfDay.Afternoon;
        }

        if (fraction < 0.90)
        {
            return TimeOfDay.Dusk;
        }

        return TimeOfDay.Night;
    }

    private sealed class WeatherSimulationState
    {
        public long CurrentSeasonId { get; set; }
        public long? CurrentWeatherEventId { get; set; }
        public int ConsecutiveUnchangedPeriods { get; set; }
        public int MinuteCounter { get; set; }
        public double CurrentTemperatureFluctuation { get; set; }
    }

    private interface ICelestialSimulation
    {
        double CurrentCelestialDay { get; }
        TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography);
        bool AdvanceByRealSeconds(double realSeconds);
    }
    private sealed class ClockDrivenCelestialSimulation : ICelestialSimulation
    {
        private readonly ICelestialObject _celestial;
        private readonly List<ClockAdvanceState> _clocks;
        private double _previousDay;

        public ClockDrivenCelestialSimulation(ICelestialObject celestial, IEnumerable<IClock> clocks)
        {
            _celestial = celestial;
            _clocks = clocks
                .Distinct()
                .Select(x => new ClockAdvanceState(x))
                .ToList();
            _previousDay = _celestial.CurrentCelestialDay;
        }

        public double CurrentCelestialDay => _celestial.CurrentCelestialDay;

        public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
        {
            return _celestial.CurrentTimeOfDay(geography);
        }

        public bool AdvanceByRealSeconds(double realSeconds)
        {
            foreach (ClockAdvanceState state in _clocks)
            {
                state.AdvanceByRealSeconds(realSeconds);
            }

            double day = _celestial.CurrentCelestialDay;
            bool wrapped = day + 0.000_001 < _previousDay;
            _previousDay = day;
            return wrapped;
        }

        private sealed class ClockAdvanceState
        {
            private readonly IClock _clock;
            private readonly double _inGameSecondsPerRealSecond;
            private double _fractionalInGameSeconds;

            public ClockAdvanceState(IClock clock)
            {
                _clock = clock;
                _inGameSecondsPerRealSecond = Math.Max(0.000_001, clock.InGameSecondsPerRealSecond);
            }

            public void AdvanceByRealSeconds(double realSeconds)
            {
                _fractionalInGameSeconds += realSeconds * _inGameSecondsPerRealSecond;
                int wholeSeconds = (int)Math.Floor(_fractionalInGameSeconds);
                if (wholeSeconds <= 0)
                {
                    return;
                }

                _fractionalInGameSeconds -= wholeSeconds;
                _clock.CurrentTime.AddSeconds(wholeSeconds);
            }
        }
    }

    private sealed class CompositeCelestialSimulation : ICelestialSimulation
    {
        private readonly ICelestialSimulation _daySource;
        private readonly ICelestialSimulation _timeSource;

        public CompositeCelestialSimulation(ICelestialSimulation daySource, ICelestialSimulation timeSource)
        {
            _daySource = daySource;
            _timeSource = timeSource;
        }

        public double CurrentCelestialDay => _daySource.CurrentCelestialDay;

        public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
        {
            return _timeSource.CurrentTimeOfDay(geography);
        }

        public bool AdvanceByRealSeconds(double realSeconds)
        {
            bool wrapped = _daySource.AdvanceByRealSeconds(realSeconds);
            if (!ReferenceEquals(_daySource, _timeSource))
            {
                _timeSource.AdvanceByRealSeconds(realSeconds);
            }

            return wrapped;
        }
    }

    private sealed class FixedNightCelestialSimulation : ICelestialSimulation
    {
        private readonly double _day;

        public FixedNightCelestialSimulation(double day = 0.0)
        {
            _day = day;
        }

        public double CurrentCelestialDay => _day;

        public TimeOfDay CurrentTimeOfDay(GeographicCoordinate geography)
        {
            return TimeOfDay.Night;
        }

        public bool AdvanceByRealSeconds(double realSeconds)
        {
            return false;
        }
    }

    private sealed class RuntimeSimulationState
    {
        private readonly FeedClockState _feedClock;
        private readonly ICelestialSimulation? _celestial;
        private readonly double _realSecondsPerFeedMinute;
        private readonly int _pseudoDayMinutes;
        private readonly double _pseudoYearLengthDays;
        private readonly GeographicCoordinate _geography;
        private bool _yearBoundary;
        private int _pseudoMinutesInDay;
        private double _pseudoDayNumber;

        public RuntimeSimulationState(
            FeedClockState feedClock,
            ICelestialSimulation? celestial,
            double realSecondsPerFeedMinute,
            double yearLengthDays,
            GeographicCoordinate geography)
        {
            _feedClock = feedClock;
            _celestial = celestial;
            _realSecondsPerFeedMinute = realSecondsPerFeedMinute;
            _pseudoDayMinutes = Math.Max(1, feedClock.HoursPerDay * feedClock.MinutesPerHour);
            _pseudoYearLengthDays = Math.Max(1.0, yearLengthDays);
            _geography = geography;
        }

        public void AdvanceOneFeedMinute()
        {
            _yearBoundary = false;
            _feedClock.AdvanceOneMinute();

            if (_celestial is not null)
            {
                _yearBoundary = _celestial.AdvanceByRealSeconds(_realSecondsPerFeedMinute);
                return;
            }

            if (++_pseudoMinutesInDay < _pseudoDayMinutes)
            {
                return;
            }

            _pseudoMinutesInDay = 0;
            _pseudoDayNumber++;
            if (_pseudoDayNumber < _pseudoYearLengthDays)
            {
                return;
            }

            _pseudoDayNumber = 0.0;
            _yearBoundary = true;
        }

        public bool ConsumeYearBoundary()
        {
            bool value = _yearBoundary;
            _yearBoundary = false;
            return value;
        }

        public double GetCelestialDay()
        {
            if (_celestial is not null)
            {
                return _celestial.CurrentCelestialDay;
            }

            return _pseudoDayNumber + (_pseudoMinutesInDay / (double)_pseudoDayMinutes);
        }

        public TimeOfDay GetTimeOfDay()
        {
            if (_celestial is not null)
            {
                return _celestial.CurrentTimeOfDay(_geography);
            }

            return DetermineTimeOfDayByHour(_feedClock.LocalHour, _feedClock.HoursPerDay);
        }

        public int GetLocalHour()
        {
            return _feedClock.LocalHour;
        }
    }

    private sealed class FeedClockState
    {
        private readonly int _primaryOffsetHours;
        private readonly int _primaryOffsetMinutes;
        private readonly int _targetOffsetHours;
        private readonly int _targetOffsetMinutes;

        private int _hours;
        private int _minutes;

        public FeedClockState(
            int hours,
            int minutes,
            int hoursPerDay,
            int minutesPerHour,
            int primaryOffsetHours,
            int primaryOffsetMinutes,
            int targetOffsetHours,
            int targetOffsetMinutes)
        {
            HoursPerDay = Math.Max(1, hoursPerDay);
            MinutesPerHour = Math.Max(1, minutesPerHour);
            _hours = Mod(hours, HoursPerDay);
            _minutes = Mod(minutes, MinutesPerHour);
            _primaryOffsetHours = primaryOffsetHours;
            _primaryOffsetMinutes = primaryOffsetMinutes;
            _targetOffsetHours = targetOffsetHours;
            _targetOffsetMinutes = targetOffsetMinutes;
        }

        public int HoursPerDay { get; }
        public int MinutesPerHour { get; }

        public int LocalHour
        {
            get
            {
                int localMinutes = _minutes + _targetOffsetMinutes - _primaryOffsetMinutes;
                int hourCarry = (int)Math.Floor(localMinutes / (double)MinutesPerHour);
                int localHours = _hours - _primaryOffsetHours + _targetOffsetHours + hourCarry;
                return Mod(localHours, HoursPerDay);
            }
        }

        public void AdvanceOneMinute()
        {
            if (++_minutes < MinutesPerHour)
            {
                return;
            }

            _minutes = 0;
            if (++_hours < HoursPerDay)
            {
                return;
            }

            _hours = 0;
        }

        private static int Mod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}







