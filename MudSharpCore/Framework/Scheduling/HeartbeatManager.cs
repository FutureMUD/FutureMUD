
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
        InvokeHeartbeat(_hourHeartbeat, "Hour");
        InvokeHeartbeat(_FuzzyHourGeneration1, "FuzzyHour1");
        InvokeHeartbeat(_FuzzyHourGeneration2, "FuzzyHour2");
        InvokeHeartbeat(_FuzzyHourGeneration3, "FuzzyHour3");
        InvokeHeartbeat(_FuzzyHourGeneration4, "FuzzyHour4");
        InvokeHeartbeat(_FuzzyHourGeneration5, "FuzzyHour5");
    }
    public void ManuallyFireHeartbeatMinute()
    {
        InvokeHeartbeat(_minuteHeartbeat, "Minute");
        InvokeHeartbeat(_FuzzyMinuteGeneration1, "FuzzyMinute1");
        InvokeHeartbeat(_FuzzyMinuteGeneration2, "FuzzyMinute2");
        InvokeHeartbeat(_FuzzyMinuteGeneration3, "FuzzyMinute3");
        InvokeHeartbeat(_FuzzyMinuteGeneration4, "FuzzyMinute4");
        InvokeHeartbeat(_FuzzyMinuteGeneration5, "FuzzyMinute5");
    }
    public void ManuallyFireHeartbeat30Second()
    {
        InvokeHeartbeat(_thirtySecondHeartbeat, "ThirtySecond");
        InvokeHeartbeat(_FuzzyThirtySecondGeneration1, "FuzzyThirtySecond1");
        InvokeHeartbeat(_FuzzyThirtySecondGeneration2, "FuzzyThirtySecond2");
        InvokeHeartbeat(_FuzzyThirtySecondGeneration3, "FuzzyThirtySecond3");
        InvokeHeartbeat(_FuzzyThirtySecondGeneration4, "FuzzyThirtySecond4");
        InvokeHeartbeat(_FuzzyThirtySecondGeneration5, "FuzzyThirtySecond5");
    }
    public void ManuallyFireHeartbeat10Second()
    {
        InvokeHeartbeat(_tenSecondHeartbeat, "TenSecond");
        InvokeHeartbeat(_FuzzyTenSecondGeneration1, "FuzzyTenSecond1");
        InvokeHeartbeat(_FuzzyTenSecondGeneration2, "FuzzyTenSecond2");
        InvokeHeartbeat(_FuzzyTenSecondGeneration3, "FuzzyTenSecond3");
        InvokeHeartbeat(_FuzzyTenSecondGeneration4, "FuzzyTenSecond4");
        InvokeHeartbeat(_FuzzyTenSecondGeneration5, "FuzzyTenSecond5");
    }
    public void ManuallyFireHeartbeat5Second()
    {
        InvokeHeartbeat(_FuzzyFiveSecondGeneration1, "FuzzyFiveSecond1");
        InvokeHeartbeat(_FuzzyFiveSecondGeneration2, "FuzzyFiveSecond2");
        InvokeHeartbeat(_FuzzyFiveSecondGeneration3, "FuzzyFiveSecond3");
        InvokeHeartbeat(_FuzzyFiveSecondGeneration4, "FuzzyFiveSecond4");
        InvokeHeartbeat(_FuzzyFiveSecondGeneration5, "FuzzyFiveSecond5");
    }
    public void ManuallyFireHeartbeatSecond()
    {
        InvokeHeartbeat(_secondHeartbeat, "Second");
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

    #region 5 Minute Heartbeat
    private event HeartbeatManagerDelegate _Fuzzy5mGeneration1;
    private event HeartbeatManagerDelegate _Fuzzy5mGeneration2;
    private event HeartbeatManagerDelegate _Fuzzy5mGeneration3;
    private event HeartbeatManagerDelegate _Fuzzy5mGeneration4;
    private event HeartbeatManagerDelegate _Fuzzy5mGeneration5;
    private int _last5mGeneration = 1;
    public event HeartbeatManagerDelegate FuzzyFiveMinuteHeartbeat
    {
        add
        {
            switch (_last5mGeneration++)
            {
                case 1:
                    _Fuzzy5mGeneration1 += value;
                    break;
                case 2:
                    _Fuzzy5mGeneration2 += value;
                    break;
                case 3:
                    _Fuzzy5mGeneration3 += value;
                    break;
                case 4:
                    _Fuzzy5mGeneration4 += value;
                    break;
                case 5:
                    _Fuzzy5mGeneration5 += value;
                    break;
            }

            if (_last5mGeneration > 5)
            {
                _last5mGeneration = 1;
            }
        }
        remove
        {
            _Fuzzy5mGeneration1 -= value;
            _Fuzzy5mGeneration2 -= value;
            _Fuzzy5mGeneration3 -= value;
            _Fuzzy5mGeneration4 -= value;
            _Fuzzy5mGeneration5 -= value;
        }
    }
    #endregion

    #region 10 Minute Heartbeat
    private event HeartbeatManagerDelegate _Fuzzy10mGeneration1;
    private event HeartbeatManagerDelegate _Fuzzy10mGeneration2;
    private event HeartbeatManagerDelegate _Fuzzy10mGeneration3;
    private event HeartbeatManagerDelegate _Fuzzy10mGeneration4;
    private event HeartbeatManagerDelegate _Fuzzy10mGeneration5;
    private int _last10mGeneration = 1;
    public event HeartbeatManagerDelegate FuzzyTenMinuteHeartbeat
    {
        add
        {
            switch (_last10mGeneration++)
            {
                case 1:
                    _Fuzzy10mGeneration1 += value;
                    break;
                case 2:
                    _Fuzzy10mGeneration2 += value;
                    break;
                case 3:
                    _Fuzzy10mGeneration3 += value;
                    break;
                case 4:
                    _Fuzzy10mGeneration4 += value;
                    break;
                case 5:
                    _Fuzzy10mGeneration5 += value;
                    break;
            }

            if (_last10mGeneration > 5)
            {
                _last10mGeneration = 1;
            }
        }
        remove
        {
            _Fuzzy10mGeneration1 -= value;
            _Fuzzy10mGeneration2 -= value;
            _Fuzzy10mGeneration3 -= value;
            _Fuzzy10mGeneration4 -= value;
            _Fuzzy10mGeneration5 -= value;
        }
    }
    #endregion

    #region 30 Minute Heartbeat
    private event HeartbeatManagerDelegate _Fuzzy30mGeneration1;
    private event HeartbeatManagerDelegate _Fuzzy30mGeneration2;
    private event HeartbeatManagerDelegate _Fuzzy30mGeneration3;
    private event HeartbeatManagerDelegate _Fuzzy30mGeneration4;
    private event HeartbeatManagerDelegate _Fuzzy30mGeneration5;
    private int _last30mGeneration = 1;
    public event HeartbeatManagerDelegate FuzzyThirtyMinuteHeartbeat
    {
        add
        {
            switch (_last30mGeneration++)
            {
                case 1:
                    _Fuzzy30mGeneration1 += value;
                    break;
                case 2:
                    _Fuzzy30mGeneration2 += value;
                    break;
                case 3:
                    _Fuzzy30mGeneration3 += value;
                    break;
                case 4:
                    _Fuzzy30mGeneration4 += value;
                    break;
                case 5:
                    _Fuzzy30mGeneration5 += value;
                    break;
            }

            if (_last30mGeneration > 5)
            {
                _last30mGeneration = 1;
            }
        }
        remove
        {
            _Fuzzy30mGeneration1 -= value;
            _Fuzzy30mGeneration2 -= value;
            _Fuzzy30mGeneration3 -= value;
            _Fuzzy30mGeneration4 -= value;
            _Fuzzy30mGeneration5 -= value;
        }
    }
    #endregion

    #region Hour Heartbeat
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
    #endregion

    private void Heartbeat()
    {
        heartbeatCount++;

        // Hard set
        InvokeHeartbeat(_secondHeartbeat, "Second");

        if (heartbeatCount % 10 == 0)
        {
            InvokeHeartbeat(_tenSecondHeartbeat, "TenSecond");
        }

        if (heartbeatCount % 30 == 0)
        {
            InvokeHeartbeat(_thirtySecondHeartbeat, "ThirtySecond");
        }

        if (heartbeatCount % 60 == 0)
        {
            InvokeHeartbeat(_minuteHeartbeat, "Minute");
        }

        if (heartbeatCount % 3600 == 0)
        {
            InvokeHeartbeat(_hourHeartbeat, "Hour");
        }

        // Fuzzy
        switch (heartbeatCount % 5)
        {
            case 0:
                InvokeHeartbeat(_FuzzyFiveSecondGeneration1, "FuzzyFiveSecond1");
                break;
            case 1:
                InvokeHeartbeat(_FuzzyFiveSecondGeneration2, "FuzzyFiveSecond2");
                break;
            case 2:
                InvokeHeartbeat(_FuzzyFiveSecondGeneration3, "FuzzyFiveSecond3");
                break;
            case 3:
                InvokeHeartbeat(_FuzzyFiveSecondGeneration4, "FuzzyFiveSecond4");
                break;
            case 4:
                InvokeHeartbeat(_FuzzyFiveSecondGeneration5, "FuzzyFiveSecond5");
                break;
        }

        switch (heartbeatCount % 10)
        {
            case 0:
                InvokeHeartbeat(_FuzzyTenSecondGeneration1, "FuzzyTenSecond1");
                break;
            case 2:
                InvokeHeartbeat(_FuzzyTenSecondGeneration2, "FuzzyTenSecond2");
                break;
            case 4:
                InvokeHeartbeat(_FuzzyTenSecondGeneration3, "FuzzyTenSecond3");
                break;
            case 6:
                InvokeHeartbeat(_FuzzyTenSecondGeneration4, "FuzzyTenSecond4");
                break;
            case 8:
                InvokeHeartbeat(_FuzzyTenSecondGeneration5, "FuzzyTenSecond5");
                break;
        }

        switch (heartbeatCount % 30)
        {
            case 0:
                InvokeHeartbeat(_FuzzyThirtySecondGeneration1, "FuzzyThirtySecond1");
                break;
            case 6:
                InvokeHeartbeat(_FuzzyThirtySecondGeneration2, "FuzzyThirtySecond2");
                break;
            case 12:
                InvokeHeartbeat(_FuzzyThirtySecondGeneration3, "FuzzyThirtySecond3");
                break;
            case 18:
                InvokeHeartbeat(_FuzzyThirtySecondGeneration4, "FuzzyThirtySecond4");
                break;
            case 24:
                InvokeHeartbeat(_FuzzyThirtySecondGeneration5, "FuzzyThirtySecond5");
                break;
        }

        switch (heartbeatCount % 60)
        {
            case 0:
                InvokeHeartbeat(_FuzzyMinuteGeneration1, "FuzzyMinute1");
                break;
            case 12:
                InvokeHeartbeat(_FuzzyMinuteGeneration2, "FuzzyMinute2");
                break;
            case 24:
                InvokeHeartbeat(_FuzzyMinuteGeneration3, "FuzzyMinute3");
                break;
            case 36:
                InvokeHeartbeat(_FuzzyMinuteGeneration4, "FuzzyMinute4");
                break;
            case 48:
                InvokeHeartbeat(_FuzzyMinuteGeneration5, "FuzzyMinute5");
                break;
        }

        switch (heartbeatCount % 300)
        {
            case 0:
                InvokeHeartbeat(_Fuzzy5mGeneration1, "FuzzyFiveMinute1");
                break;
            case 60:
                InvokeHeartbeat(_Fuzzy5mGeneration2, "FuzzyFiveMinute2");
                break;
            case 120:
                InvokeHeartbeat(_Fuzzy5mGeneration3, "FuzzyFiveMinute3");
                break;
            case 180:
                InvokeHeartbeat(_Fuzzy5mGeneration4, "FuzzyFiveMinute4");
                break;
            case 240:
                InvokeHeartbeat(_Fuzzy5mGeneration5, "FuzzyFiveMinute5");
                break;
        }

        switch (heartbeatCount % 600)
        {
            case 0:
                InvokeHeartbeat(_Fuzzy10mGeneration1, "FuzzyTenMinute1");
                break;
            case 120:
                InvokeHeartbeat(_Fuzzy10mGeneration2, "FuzzyTenMinute2");
                break;
            case 240:
                InvokeHeartbeat(_Fuzzy10mGeneration3, "FuzzyTenMinute3");
                break;
            case 360:
                InvokeHeartbeat(_Fuzzy10mGeneration4, "FuzzyTenMinute4");
                break;
            case 480:
                InvokeHeartbeat(_Fuzzy10mGeneration5, "FuzzyTenMinute5");
                break;
        }

        switch (heartbeatCount % 1800)
        {
            case 0:
                InvokeHeartbeat(_Fuzzy30mGeneration1, "FuzzyThirtyMinute1");
                break;
            case 360:
                InvokeHeartbeat(_Fuzzy30mGeneration2, "FuzzyThirtyMinute2");
                break;
            case 720:
                InvokeHeartbeat(_Fuzzy30mGeneration3, "FuzzyThirtyMinute3");
                break;
            case 1080:
                InvokeHeartbeat(_Fuzzy30mGeneration4, "FuzzyThirtyMinute4");
                break;
            case 1440:
                InvokeHeartbeat(_Fuzzy30mGeneration5, "FuzzyThirtyMinute5");
                break;
        }

        switch (heartbeatCount % 3600)
        {
            case 0:
                InvokeHeartbeat(_FuzzyHourGeneration1, "FuzzyHour1");
                break;
            case 720:
                InvokeHeartbeat(_FuzzyHourGeneration2, "FuzzyHour2");
                break;
            case 1440:
                InvokeHeartbeat(_FuzzyHourGeneration3, "FuzzyHour3");
                break;
            case 2160:
                InvokeHeartbeat(_FuzzyHourGeneration4, "FuzzyHour4");
                break;
            case 2880:
                InvokeHeartbeat(_FuzzyHourGeneration5, "FuzzyHour5");
                break;
        }
    }

	public void AppendPerformanceReport(System.Text.StringBuilder sb)
	{
		sb.AppendLine("Heartbeat subscribers:");
		AppendSubscriberCount(sb, "Second", _secondHeartbeat);
		AppendSubscriberCount(sb, "Ten Second", _tenSecondHeartbeat);
		AppendSubscriberCount(sb, "Thirty Second", _thirtySecondHeartbeat);
		AppendSubscriberCount(sb, "Minute", _minuteHeartbeat);
		AppendSubscriberCount(sb, "Hour", _hourHeartbeat);
		AppendSubscriberCount(sb, "Fuzzy Five Second", _FuzzyFiveSecondGeneration1, _FuzzyFiveSecondGeneration2,
			_FuzzyFiveSecondGeneration3, _FuzzyFiveSecondGeneration4, _FuzzyFiveSecondGeneration5);
		AppendSubscriberCount(sb, "Fuzzy Ten Second", _FuzzyTenSecondGeneration1, _FuzzyTenSecondGeneration2,
			_FuzzyTenSecondGeneration3, _FuzzyTenSecondGeneration4, _FuzzyTenSecondGeneration5);
		AppendSubscriberCount(sb, "Fuzzy Thirty Second", _FuzzyThirtySecondGeneration1, _FuzzyThirtySecondGeneration2,
			_FuzzyThirtySecondGeneration3, _FuzzyThirtySecondGeneration4, _FuzzyThirtySecondGeneration5);
		AppendSubscriberCount(sb, "Fuzzy Minute", _FuzzyMinuteGeneration1, _FuzzyMinuteGeneration2,
			_FuzzyMinuteGeneration3, _FuzzyMinuteGeneration4, _FuzzyMinuteGeneration5);
	}

	private void InvokeHeartbeat(HeartbeatManagerDelegate callbacks, string cadence)
	{
		if (callbacks is null)
		{
			return;
		}

		var monitor = (Gameworld as MudSharp.Framework.Diagnostics.IRuntimePerformanceMonitorProvider)
			?.RuntimePerformanceMonitor;
		if (monitor?.Enabled != true)
		{
			callbacks();
			return;
		}

		foreach (var callback in Delegate.EnumerateInvocationList(callbacks))
		{
			var started = System.Diagnostics.Stopwatch.GetTimestamp();
			callback();
			monitor.RecordHeartbeatCallback(cadence, callback,
				System.Diagnostics.Stopwatch.GetTimestamp() - started);
		}
	}

	private static void AppendSubscriberCount(System.Text.StringBuilder sb, string name,
		params HeartbeatManagerDelegate[] callbacks)
	{
		var count = 0;
		foreach (var callbackList in callbacks)
		{
			if (callbackList is null)
			{
				continue;
			}

			foreach (var _ in Delegate.EnumerateInvocationList(callbackList))
			{
				count++;
			}
		}

		sb.AppendLine($"\t{name}: {count:N0}");
	}
}
