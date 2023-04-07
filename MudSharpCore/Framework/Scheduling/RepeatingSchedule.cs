using System;

namespace MudSharp.Framework.Scheduling;

public class RepeatingSchedule : Schedule
{
	protected int _repetitions;
	protected int _targetRepetitions;

	public RepeatingSchedule(IFuturemud game, Action action, ScheduleType type, TimeSpan duration,
		string debugDescription,
		int numberOfRepetitions = -1)
		: base(action, type, duration, debugDescription)
	{
		_game = game;
		_duration = duration;
	}

	protected IFuturemud _game { get; set; }
	protected TimeSpan _duration { get; set; }

	public override void Fire()
	{
		Action();
		if (_targetRepetitions == 0 || _repetitions++ != _targetRepetitions)
		{
			TriggerETA += _duration;
			_game.Scheduler.AddSchedule(this);
		}
	}
}

public class RepeatingSchedule<T> : Schedule<T> where T : class
{
	protected int _repetitions;
	protected int _targetRepetitions;

	public RepeatingSchedule(T parameter1, IFuturemud game, Action<T> action, ScheduleType type, TimeSpan duration,
		string debugDescription,
		int numberOfRepetitions = -1)
		: base(parameter1, action, type, duration, debugDescription)
	{
		_game = game;
		_duration = duration;
	}

	protected IFuturemud _game { get; set; }
	protected TimeSpan _duration { get; set; }

	public override void Fire()
	{
		Action(Parameter1);
		if (_targetRepetitions == 0 || _repetitions++ != _targetRepetitions)
		{
			TriggerETA += _duration;
			_game.Scheduler.AddSchedule(this);
		}
	}
}

public class RepeatingSchedule<T1, T2> : Schedule<T1, T2> where T1 : class where T2 : class
{
	protected int _repetitions;
	protected int _targetRepetitions;

	public RepeatingSchedule(T1 parameter1, T2 parameter2, IFuturemud game, Action<T1, T2> action, ScheduleType type,
		string debugDescription,
		TimeSpan duration, int numberOfRepetitions = -1)
		: base(parameter1, parameter2, action, type, duration, debugDescription)
	{
		_game = game;
		_duration = duration;
	}

	protected IFuturemud _game { get; set; }
	protected TimeSpan _duration { get; set; }

	public override void Fire()
	{
		Action(Parameter1, Parameter2);
		if (_targetRepetitions == 0 || _repetitions++ != _targetRepetitions)
		{
			TriggerETA += _duration;
			_game.Scheduler.AddSchedule(this);
		}
	}
}