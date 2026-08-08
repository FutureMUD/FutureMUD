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
			var iterations = 0;
			while (state.NextUpdateUtc <= now)
			{
				iterations++;
				state.Clock.CurrentTime.AddSeconds(1);
				state.NextUpdateUtc = state.NextUpdateUtc.AddMilliseconds(1000.0 / state.Clock.InGameSecondsPerRealSecond);
			}

			if (state.NextUpdateUtc < _nextUpdateUtc)
			{
				_nextUpdateUtc = state.NextUpdateUtc;
			}
			if (iterations > 10)
			{
				Console.WriteLine($"The clock ended up taking {iterations:N0} iterations to update.");
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
			var next = now.AddMilliseconds(1000.0 / clock.InGameSecondsPerRealSecond);
			_clocks.Add(new ClockState(clock, next));
			if (next < _nextUpdateUtc)
			{
				_nextUpdateUtc = next;
			}
		}
	}

	private DateTime UtcNow => _timeProvider.GetUtcNow().UtcDateTime;

	private sealed class ClockState(IClock clock, DateTime nextUpdateUtc)
	{
		public IClock Clock { get; } = clock;
		public DateTime NextUpdateUtc { get; set; } = nextUpdateUtc;
	}
}
