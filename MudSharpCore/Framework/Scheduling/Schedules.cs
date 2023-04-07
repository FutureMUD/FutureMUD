using System;
using MudSharp.Character;

namespace MudSharp.Framework.Scheduling;

public abstract class ScheduleBase : IComparable<ScheduleBase>, ISchedule
{
	protected ScheduleBase(ScheduleType type, int days = 0, int hours = 0, int minutes = 0, int seconds = 0,
		int milliseconds = 0)
	{
		Type = type;
		CreatedAt = DateTime.UtcNow;
		Duration = new TimeSpan(days, hours, minutes, seconds, milliseconds);
		TriggerETA = CreatedAt + Duration;
	}

	protected ScheduleBase(ScheduleType type, TimeSpan duration)
	{
		Type = type;
		CreatedAt = DateTime.UtcNow;
		Duration = duration;
		TriggerETA = CreatedAt + Duration;
	}

	public ScheduleType Type { get; protected set; }

	public DateTime CreatedAt { get; protected set; }

	public TimeSpan Duration { get; protected set; }

	public DateTime TriggerETA { get; set; }

	public virtual int CompareTo(ScheduleBase other)
	{
		return other.TriggerETA.CompareTo(TriggerETA);
	}

	public abstract void Fire();

	public static TimeSpan MakeSpan(int days = 0, int hours = 0, int minutes = 0, int seconds = 0,
		int milliseconds = 0)
	{
		return new TimeSpan(days, hours, minutes, seconds, milliseconds);
	}

	public override string ToString()
	{
		return "This schedule is of type " + Type + ".\n" + "It will be fired at " + TriggerETA + ".";
	}

	public virtual bool PertainsTo(IFrameworkItem item)
	{
		return false;
	}

	public virtual bool PertainsTo(IFrameworkItem item, ScheduleType type)
	{
		return PertainsTo(item) && Type == type;
	}

	public abstract string DebugInfoString { get; }
}

public class CommandSchedule : ScheduleBase
{
	public CommandSchedule(string command, ICharacter actor, ScheduleType type, TimeSpan duration)
		: base(type, duration)
	{
		Actor = actor;
		Command = command;
	}

	public ICharacter Actor { get; protected set; }

	public string Command { get; protected set; }

	public override void Fire()
	{
		Actor.OutOfContextExecuteCommand(Command);
	}

	public override bool PertainsTo(IFrameworkItem item)
	{
		return Actor == item;
	}

	public override string DebugInfoString =>
		$"CommandSchedule {Actor.HowSeen(null, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)} executes {Command.Colour(Telnet.Yellow)}";
}

public class Schedule : ScheduleBase
{
	public Schedule(Action action, ScheduleType type, TimeSpan duration, string debugDescription)
		: base(type, duration)
	{
		Action = action;
		DebugDescription = debugDescription;
	}

	public Action Action { get; protected set; }

	public string DebugDescription { get; protected set; }

	public override void Fire()
	{
		Action();
	}

	public override string DebugInfoString => $"Action {DebugDescription.Colour(Telnet.Cyan)}";
}

public class Schedule<T1> : ScheduleBase where T1 : class
{
	public Schedule(T1 parameter1, Action<T1> action, ScheduleType type, TimeSpan duration, string debugDescription)
		: base(type, duration)
	{
		Parameter1 = parameter1;
		DebugDescription = debugDescription;
		Action = action;
	}

	public T1 Parameter1 { get; protected set; }

	public Action<T1> Action { get; protected set; }

	public string DebugDescription { get; protected set; }

	public override string DebugInfoString => $"Action {DebugDescription}";

	public override void Fire()
	{
		Action(Parameter1);
	}

	public override bool PertainsTo(IFrameworkItem item)
	{
		return Parameter1 == item;
	}
}

public class Schedule<T1, T2> : ScheduleBase where T1 : class where T2 : class
{
	public Schedule(T1 parameter1, T2 parameter2, Action<T1, T2> action, ScheduleType type, TimeSpan duration,
		string debugDescription)
		: base(type, duration)
	{
		Parameter1 = parameter1;
		Parameter2 = parameter2;
		DebugDescription = debugDescription;
		Action = action;
	}

	public T1 Parameter1 { get; protected set; }

	public T2 Parameter2 { get; protected set; }

	public Action<T1, T2> Action { get; protected set; }

	public string DebugDescription { get; protected set; }

	public override string DebugInfoString => $"Action {DebugDescription}";

	public override void Fire()
	{
		Action(Parameter1, Parameter2);
	}

	public override bool PertainsTo(IFrameworkItem item)
	{
		return Parameter1 == item || Parameter2 == item;
	}
}