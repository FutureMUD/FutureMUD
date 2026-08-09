namespace MudSharp.TimeAndDate.Time;

public class ClockManager : IClockManager
{
	private readonly List<ClockState> _clocks = [];
	private readonly TimeProvider _timeProvider;
	private DateTime _nextUpdateUtc = DateTime.MaxValue;
	private bool _timeIsFrozen;

	public ClockManager(IFuturemud game, TimeProvider timeProvider = null)
	{
		Gameworld = game;
		_timeProvider = timeProvider ?? TimeProvider.System;
	}

	public IFuturemud Gameworld { get; protected set; }

	public void UpdateClocks()
	{
		if (_timeIsFrozen)
		{
			return;
		}

		var now = UtcNow;
		if (now < _nextUpdateUtc)
		{
			return;
		}

		_nextUpdateUtc = DateTime.MaxValue;
		foreach (var state in _clocks)
		{
			if (!double.IsFinite(state.Clock.InGameSecondsPerRealSecond) ||
			    state.Clock.InGameSecondsPerRealSecond <= 0.0)
			{
				Gameworld.SystemMessage($"Clock {state.Clock.Name} has an invalid in-game speed and was skipped.", true);
				continue;
			}

			if (now < state.NextUpdateUtc)
			{
				if (state.NextUpdateUtc < _nextUpdateUtc)
				{
					_nextUpdateUtc = state.NextUpdateUtc;
				}

				continue;
			}

			var overdueTicks = 1L + (now.Ticks - state.NextUpdateUtc.Ticks) / state.UpdateInterval.Ticks;
			for (var iteration = 0L; iteration < overdueTicks; iteration++)
			{
				state.Clock.CurrentTime.AddSeconds(1);
			}
			state.NextUpdateUtc = state.NextUpdateUtc.AddTicks(checked(state.UpdateInterval.Ticks * overdueTicks));

			if (state.NextUpdateUtc < _nextUpdateUtc)
			{
				_nextUpdateUtc = state.NextUpdateUtc;
			}
			if (overdueTicks > 10)
			{
				Gameworld.SystemMessage($"Clock {state.Clock.Name} replayed {overdueTicks:N0} overdue in-game seconds.", true);
			}
		}
	}

	public void Initialise()
	{
		_timeIsFrozen = Gameworld.GetStaticBool("TimeIsFrozen");
		if (_timeIsFrozen)
		{
			return;
		}

		PopulateClocks();
	}

	public void FreezeTime()
	{
		_timeIsFrozen = true;
		Gameworld.UpdateStaticConfiguration("TimeIsFrozen", "true");
		_clocks.Clear();
		_nextUpdateUtc = DateTime.MaxValue;
	}

	public void UnfreezeTime()
	{
		_timeIsFrozen = false;
		Gameworld.UpdateStaticConfiguration("TimeIsFrozen", "false");
		PopulateClocks();
	}

	private void PopulateClocks()
	{
		_clocks.Clear();
		_nextUpdateUtc = DateTime.MaxValue;
		var now = UtcNow;
		foreach (var clock in Gameworld.Clocks)
		{
			if (!double.IsFinite(clock.InGameSecondsPerRealSecond) || clock.InGameSecondsPerRealSecond <= 0.0)
			{
				Gameworld.SystemMessage($"Clock {clock.Name} has an invalid in-game speed and was not started.", true);
				continue;
			}

			TimeSpan interval;
			try
			{
				interval = TimeSpan.FromSeconds(1.0 / clock.InGameSecondsPerRealSecond);
			}
			catch (OverflowException)
			{
				Gameworld.SystemMessage($"Clock {clock.Name} has an in-game speed outside the scheduler range and was not started.", true);
				continue;
			}
			if (interval.Ticks <= 0)
			{
				Gameworld.SystemMessage($"Clock {clock.Name} advances too quickly for the scheduler resolution and was not started.", true);
				continue;
			}

			var next = now.AddTicks(interval.Ticks);
			_clocks.Add(new ClockState(clock, next, interval));
			if (next < _nextUpdateUtc)
			{
				_nextUpdateUtc = next;
			}
		}
	}

	private DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;

	private sealed class ClockState(IClock clock, DateTime nextUpdateUtc, TimeSpan updateInterval)
	{
		public IClock Clock { get; } = clock;
		public DateTime NextUpdateUtc { get; set; } = nextUpdateUtc;
		public TimeSpan UpdateInterval { get; } = updateInterval;
	}
}
