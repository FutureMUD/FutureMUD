using System;

namespace MudSharp.Framework.Scheduling;

public class HeartbeatManager : IHaveFuturemud, IHeartbeatManager
{
	private HeartbeatManagerDelegate _hourHeartbeat;
	private HeartbeatManagerDelegate _minuteHeartbeat;
	private HeartbeatManagerDelegate _secondHeartbeat;
	private HeartbeatManagerDelegate _tenSecondHeartbeat;
	private HeartbeatManagerDelegate _thirtySecondHeartbeat;

	private uint heartbeatCount;

	public HeartbeatManager(IFuturemud gameworld)
	{
		Gameworld = gameworld;
	}

	public void StartHeartbeatTick()
	{
		Gameworld.Scheduler.AddSchedule(new RepeatingSchedule(Gameworld, Heartbeat, ScheduleType.System,
			TimeSpan.FromSeconds(1), "Heatbeat Tick"));
	}

	public void ManuallyFireHeartbeatHour()
	{
		_hourHeartbeat?.Invoke();
		_FuzzyHourGeneration1?.Invoke();
		_FuzzyHourGeneration2?.Invoke();
		_FuzzyHourGeneration3?.Invoke();
		_FuzzyHourGeneration4?.Invoke();
		_FuzzyHourGeneration5?.Invoke();
	}
	public void ManuallyFireHeartbeatMinute()
	{
		_minuteHeartbeat?.Invoke();
		_FuzzyMinuteGeneration1?.Invoke();
		_FuzzyMinuteGeneration2?.Invoke();
		_FuzzyMinuteGeneration3?.Invoke();
		_FuzzyMinuteGeneration4?.Invoke();
		_FuzzyMinuteGeneration5?.Invoke();
	}
	public void ManuallyFireHeartbeat30Second()
	{
		_thirtySecondHeartbeat?.Invoke();
		_FuzzyThirtySecondGeneration1?.Invoke();
		_FuzzyThirtySecondGeneration2?.Invoke();
		_FuzzyThirtySecondGeneration3?.Invoke();
		_FuzzyThirtySecondGeneration4?.Invoke();
		_FuzzyThirtySecondGeneration5?.Invoke();
	}
	public void ManuallyFireHeartbeat10Second()
	{
		_tenSecondHeartbeat?.Invoke();
		_FuzzyTenSecondGeneration1?.Invoke();
		_FuzzyTenSecondGeneration2?.Invoke();
		_FuzzyTenSecondGeneration3?.Invoke();
		_FuzzyTenSecondGeneration4?.Invoke();
		_FuzzyTenSecondGeneration5?.Invoke();
	}
	public void ManuallyFireHeartbeat5Second()
	{
		_FuzzyFiveSecondGeneration1?.Invoke();
		_FuzzyFiveSecondGeneration2?.Invoke();
		_FuzzyFiveSecondGeneration3?.Invoke();
		_FuzzyFiveSecondGeneration4?.Invoke();
		_FuzzyFiveSecondGeneration5?.Invoke();
	}
	public void ManuallyFireHeartbeatSecond()
	{
		_secondHeartbeat?.Invoke();
	}

	#region IHaveGame Members

	public IFuturemud Gameworld { get; }

	#endregion

	/// <summary>
	///     The SecondHeartbeat fires approximately once every second. It does not allow the same delegate to be subscribed
	///     more than once.
	/// </summary>
	public event HeartbeatManagerDelegate SecondHeartbeat
	{
		add
		{
			if (_secondHeartbeat != null && value != null)
			{
				_secondHeartbeat -= value;
			}

			_secondHeartbeat += value;
		}
		remove => _secondHeartbeat -= value;
	}

	/// <summary>
	///     The TenSecondHeartbeat fires approximately once every 10 seconds. It does not allow the same delegate to be
	///     subscribed more than once.
	/// </summary>
	public event HeartbeatManagerDelegate TenSecondHeartbeat
	{
		add
		{
			if (_tenSecondHeartbeat != null && value != null)
			{
				_tenSecondHeartbeat -= value;
			}

			_tenSecondHeartbeat += value;
		}
		remove => _tenSecondHeartbeat -= value;
	}

	/// <summary>
	/// The ThirtySecondHeartbeat fires approximately once every 30 seconds. It does not allow the same delegate to be subscribed more than once.
	/// </summary>
	public event HeartbeatManagerDelegate ThirtySecondHeartbeat
	{
		add
		{
			if (_thirtySecondHeartbeat != null && value != null)
			{
				_thirtySecondHeartbeat -= value;
			}

			_thirtySecondHeartbeat += value;
		}
		remove => _thirtySecondHeartbeat -= value;
	}

	/// <summary>
	///     The MinuteHeartbeat fires approximately once every minute. It does not allow the same delegate to be subscribed
	///     more than once.
	/// </summary>
	public event HeartbeatManagerDelegate MinuteHeartbeat
	{
		add
		{
			if (_minuteHeartbeat != null && value != null)
			{
				_minuteHeartbeat -= value;
			}

			_minuteHeartbeat += value;
		}
		remove => _minuteHeartbeat -= value;
	}

	/// <summary>
	///     The HourHeartbeat fires approximately once every hour. It does not allow the same delegate to be subscribed more
	///     than once.
	/// </summary>
	public event HeartbeatManagerDelegate HourHeartbeat
	{
		add
		{
			if (_hourHeartbeat != null && value != null)
			{
				_hourHeartbeat -= value;
			}

			_hourHeartbeat += value;
		}
		remove => _hourHeartbeat -= value;
	}

	private event HeartbeatManagerDelegate _FuzzyFiveSecondGeneration1;
	private event HeartbeatManagerDelegate _FuzzyFiveSecondGeneration2;
	private event HeartbeatManagerDelegate _FuzzyFiveSecondGeneration3;
	private event HeartbeatManagerDelegate _FuzzyFiveSecondGeneration4;
	private event HeartbeatManagerDelegate _FuzzyFiveSecondGeneration5;
	private int _lastFiveSecondGeneration = 1;

	/// <summary>
	/// The FuzzyFiveSecondHeartbeat fires every 5 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 5 second intervals
	/// </summary>
	public event HeartbeatManagerDelegate FuzzyFiveSecondHeartbeat
	{
		add
		{
			switch (_lastFiveSecondGeneration++)
			{
				case 1:
					_FuzzyFiveSecondGeneration1 += value;
					break;
				case 2:
					_FuzzyFiveSecondGeneration2 += value;
					break;
				case 3:
					_FuzzyFiveSecondGeneration3 += value;
					break;
				case 4:
					_FuzzyFiveSecondGeneration4 += value;
					break;
				case 5:
					_FuzzyFiveSecondGeneration5 += value;
					break;
			}

			if (_lastFiveSecondGeneration > 5)
			{
				_lastFiveSecondGeneration = 1;
			}
		}
		remove
		{
			_FuzzyFiveSecondGeneration1 -= value;
			_FuzzyFiveSecondGeneration2 -= value;
			_FuzzyFiveSecondGeneration3 -= value;
			_FuzzyFiveSecondGeneration4 -= value;
			_FuzzyFiveSecondGeneration5 -= value;
		}
	}

	private event HeartbeatManagerDelegate _FuzzyTenSecondGeneration1;
	private event HeartbeatManagerDelegate _FuzzyTenSecondGeneration2;
	private event HeartbeatManagerDelegate _FuzzyTenSecondGeneration3;
	private event HeartbeatManagerDelegate _FuzzyTenSecondGeneration4;
	private event HeartbeatManagerDelegate _FuzzyTenSecondGeneration5;
	private int _lastTenSecondGeneration = 1;

	/// <summary>
	/// The FuzzyTenSecondHeartbeat fires every 10 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 10 second intervals
	/// </summary>
	public event HeartbeatManagerDelegate FuzzyTenSecondHeartbeat
	{
		add
		{
			switch (_lastTenSecondGeneration++)
			{
				case 1:
					_FuzzyTenSecondGeneration1 += value;
					break;
				case 2:
					_FuzzyTenSecondGeneration2 += value;
					break;
				case 3:
					_FuzzyTenSecondGeneration3 += value;
					break;
				case 4:
					_FuzzyTenSecondGeneration4 += value;
					break;
				case 5:
					_FuzzyTenSecondGeneration5 += value;
					break;
			}

			if (_lastTenSecondGeneration > 5)
			{
				_lastTenSecondGeneration = 1;
			}
		}
		remove
		{
			_FuzzyTenSecondGeneration1 -= value;
			_FuzzyTenSecondGeneration2 -= value;
			_FuzzyTenSecondGeneration3 -= value;
			_FuzzyTenSecondGeneration4 -= value;
			_FuzzyTenSecondGeneration5 -= value;
		}
	}

	public event HeartbeatManagerDelegate FuzzyThirtySecondHeartbeat
	{
		add
		{
			switch (_lastThirtySecondGeneration++)
			{
				case 1:
					_FuzzyThirtySecondGeneration1 += value;
					break;
				case 2:
					_FuzzyThirtySecondGeneration2 += value;
					break;
				case 3:
					_FuzzyThirtySecondGeneration3 += value;
					break;
				case 4:
					_FuzzyThirtySecondGeneration4 += value;
					break;
				case 5:
					_FuzzyThirtySecondGeneration5 += value;
					break;
			}

			if (_lastThirtySecondGeneration > 5)
			{
				_lastThirtySecondGeneration = 1;
			}
		}
		remove
		{
			_FuzzyThirtySecondGeneration1 -= value;
			_FuzzyThirtySecondGeneration2 -= value;
			_FuzzyThirtySecondGeneration3 -= value;
			_FuzzyThirtySecondGeneration4 -= value;
			_FuzzyThirtySecondGeneration5 -= value;
		}
	}

	private event HeartbeatManagerDelegate _FuzzyThirtySecondGeneration1;
	private event HeartbeatManagerDelegate _FuzzyThirtySecondGeneration2;
	private event HeartbeatManagerDelegate _FuzzyThirtySecondGeneration3;
	private event HeartbeatManagerDelegate _FuzzyThirtySecondGeneration4;
	private event HeartbeatManagerDelegate _FuzzyThirtySecondGeneration5;
	private int _lastThirtySecondGeneration = 1;

	private event HeartbeatManagerDelegate _FuzzyMinuteGeneration1;
	private event HeartbeatManagerDelegate _FuzzyMinuteGeneration2;
	private event HeartbeatManagerDelegate _FuzzyMinuteGeneration3;
	private event HeartbeatManagerDelegate _FuzzyMinuteGeneration4;
	private event HeartbeatManagerDelegate _FuzzyMinuteGeneration5;
	private int _lastMinuteGeneration = 1;

	/// <summary>
	/// The FuzzyMinuteHeartbeat fires every 60 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 60 second intervals
	/// </summary>
	public event HeartbeatManagerDelegate FuzzyMinuteHeartbeat
	{
		add
		{
			switch (_lastMinuteGeneration++)
			{
				case 1:
					_FuzzyMinuteGeneration1 += value;
					break;
				case 2:
					_FuzzyMinuteGeneration2 += value;
					break;
				case 3:
					_FuzzyMinuteGeneration3 += value;
					break;
				case 4:
					_FuzzyMinuteGeneration4 += value;
					break;
				case 5:
					_FuzzyMinuteGeneration5 += value;
					break;
			}

			if (_lastMinuteGeneration > 5)
			{
				_lastMinuteGeneration = 1;
			}
		}
		remove
		{
			_FuzzyMinuteGeneration1 -= value;
			_FuzzyMinuteGeneration2 -= value;
			_FuzzyMinuteGeneration3 -= value;
			_FuzzyMinuteGeneration4 -= value;
			_FuzzyMinuteGeneration5 -= value;
		}
	}

	private event HeartbeatManagerDelegate _FuzzyHourGeneration1;
	private event HeartbeatManagerDelegate _FuzzyHourGeneration2;
	private event HeartbeatManagerDelegate _FuzzyHourGeneration3;
	private event HeartbeatManagerDelegate _FuzzyHourGeneration4;
	private event HeartbeatManagerDelegate _FuzzyHourGeneration5;
	private int _lastHourGeneration = 1;

	/// <summary>
	/// The FuzzyHourHeartbeat fires every 3600 seconds but splits up subscribers into different generations so that they are approximately "load balanced" and not all fire at the same 3600 second intervals
	/// </summary>
	public event HeartbeatManagerDelegate FuzzyHourHeartbeat
	{
		add
		{
			switch (_lastHourGeneration++)
			{
				case 1:
					_FuzzyHourGeneration1 += value;
					break;
				case 2:
					_FuzzyHourGeneration2 += value;
					break;
				case 3:
					_FuzzyHourGeneration3 += value;
					break;
				case 4:
					_FuzzyHourGeneration4 += value;
					break;
				case 5:
					_FuzzyHourGeneration5 += value;
					break;
			}

			if (_lastHourGeneration > 5)
			{
				_lastHourGeneration = 1;
			}
		}
		remove
		{
			_FuzzyHourGeneration1 -= value;
			_FuzzyHourGeneration2 -= value;
			_FuzzyHourGeneration3 -= value;
			_FuzzyHourGeneration4 -= value;
			_FuzzyHourGeneration5 -= value;
		}
	}

	private void Heartbeat()
	{
		heartbeatCount++;

		// Hard set
		_secondHeartbeat?.Invoke();

		if (heartbeatCount % 10 == 0)
		{
			_tenSecondHeartbeat?.Invoke();
		}

		if (heartbeatCount % 30 == 0)
		{
			_thirtySecondHeartbeat?.Invoke();
		}

		if (heartbeatCount % 60 == 0)
		{
			_minuteHeartbeat?.Invoke();
		}

		if (heartbeatCount % 3600 == 0)
		{
			_hourHeartbeat?.Invoke();
		}

		// Fuzzy
		switch (heartbeatCount % 5)
		{
			case 0:
				_FuzzyFiveSecondGeneration1?.Invoke();
				break;
			case 1:
				_FuzzyFiveSecondGeneration2?.Invoke();
				break;
			case 2:
				_FuzzyFiveSecondGeneration3?.Invoke();
				break;
			case 3:
				_FuzzyFiveSecondGeneration4?.Invoke();
				break;
			case 4:
				_FuzzyFiveSecondGeneration5?.Invoke();
				break;
		}

		switch (heartbeatCount % 10)
		{
			case 0:
				_FuzzyTenSecondGeneration1?.Invoke();
				break;
			case 2:
				_FuzzyTenSecondGeneration2?.Invoke();
				break;
			case 4:
				_FuzzyTenSecondGeneration3?.Invoke();
				break;
			case 6:
				_FuzzyTenSecondGeneration4?.Invoke();
				break;
			case 8:
				_FuzzyTenSecondGeneration5?.Invoke();
				break;
		}

		switch (heartbeatCount % 30)
		{
			case 0:
				_FuzzyThirtySecondGeneration1?.Invoke();
				break;
			case 6:
				_FuzzyThirtySecondGeneration2?.Invoke();
				break;
			case 12:
				_FuzzyThirtySecondGeneration3?.Invoke();
				break;
			case 18:
				_FuzzyThirtySecondGeneration4?.Invoke();
				break;
			case 24:
				_FuzzyThirtySecondGeneration5?.Invoke();
				break;
		}

		switch (heartbeatCount % 60)
		{
			case 0:
				_FuzzyMinuteGeneration1?.Invoke();
				break;
			case 12:
				_FuzzyMinuteGeneration2?.Invoke();
				break;
			case 24:
				_FuzzyMinuteGeneration3?.Invoke();
				break;
			case 36:
				_FuzzyMinuteGeneration4?.Invoke();
				break;
			case 48:
				_FuzzyMinuteGeneration5?.Invoke();
				break;
		}

		switch (heartbeatCount % 3600)
		{
			case 0:
				_FuzzyHourGeneration1?.Invoke();
				break;
			case 720:
				_FuzzyHourGeneration2?.Invoke();
				break;
			case 1440:
				_FuzzyHourGeneration3?.Invoke();
				break;
			case 2160:
				_FuzzyHourGeneration4?.Invoke();
				break;
			case 2880:
				_FuzzyHourGeneration5?.Invoke();
				break;
		}
	}
}