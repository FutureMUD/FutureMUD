using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time;

public class ClockManager : IClockManager
{
	private readonly Dictionary<IClock, DateTime> _clocks = new();

	public ClockManager(IFuturemud game)
	{
		Gameworld = game;
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; protected set; }

	#endregion

	public void UpdateClocks()
	{
		if (_timeIsFrozen)
		{
			return;
		}

		foreach (var clock in _clocks.Keys.ToList())
		{
			var iterations = 0;
			while (_clocks[clock] <= DateTime.UtcNow)
			{
				iterations++;
				clock.CurrentTime.AddSeconds(1);
				_clocks[clock] = _clocks[clock].AddMilliseconds(1000.0 / clock.InGameSecondsPerRealSecond);
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
		
		foreach (var clock in Gameworld.Clocks)
		{
			_clocks.Add(clock, DateTime.UtcNow.AddMilliseconds(1000.0 / clock.InGameSecondsPerRealSecond));
		}
	}

	private bool _timeIsFrozen = false;

	public void FreezeTime()
	{
		_timeIsFrozen = true;
		Gameworld.UpdateStaticConfiguration("TimeIsFrozen", "true");
		_clocks.Clear();
	}

	public void UnfreezeTime()
	{
		_timeIsFrozen = false;
		Gameworld.UpdateStaticConfiguration("TimeIsFrozen", "false");
		_clocks.Clear();
		foreach (var clock in Gameworld.Clocks)
		{
			_clocks.Add(clock, DateTime.UtcNow.AddMilliseconds(1000.0 / clock.InGameSecondsPerRealSecond));
		}
	}
}