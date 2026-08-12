#nullable enable

using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.GameItems.Components;

internal static class ClockDetonatorScheduleEvaluator
{
	internal static bool IsDue(MudInstant target, MudInstant current)
	{
		return current >= target;
	}

	internal static bool CanRetainArmedTarget(long oldCalendarId, long oldClockId, long newCalendarId,
		long newClockId)
	{
		return oldCalendarId == newCalendarId && oldClockId == newClockId;
	}
}

public class ClockDetonatorGameItemComponent : GameItemComponent, IArmableExplosiveTrigger
{
	private ClockDetonatorGameItemComponentProto _prototype;
	private MudInstant? _targetInstant;
	private bool _clockSubscribed;
	private bool _runtimeActive;

	public ClockDetonatorGameItemComponent(ClockDetonatorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ClockDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ClockDetonatorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ClockDetonatorGameItemComponent(ClockDetonatorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_targetInstant = rhs._targetInstant;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public bool Armed => _targetInstant is { IsNever: false };

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		RemoveClockSubscription();
		var newClockPrototype = (ClockDetonatorGameItemComponentProto)newProto;
		if (Armed && !ClockDetonatorScheduleEvaluator.CanRetainArmedTarget(
			    _prototype.Calendar.Id, _prototype.Clock.Id, newClockPrototype.Calendar.Id,
			    newClockPrototype.Clock.Id))
		{
			// The stored MudInstant was authored against the old clock. Raw ticks from different
			// clocks are not safely comparable, so an incompatible component update fails safe.
			_targetInstant = null;
			Changed = true;
		}

		_prototype = newClockPrototype;
		if (_runtimeActive && Armed)
		{
			EnsureClockSubscription();
			CheckTargetTime();
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ClockDetonatorGameItemComponent(this, newParent, temporary);
	}

	private void LoadFromXml(XElement root)
	{
		if (MudInstant.TryParse(root.Element("TargetInstant")?.Value, out var instant) && !instant.IsNever)
		{
			_targetInstant = instant;
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TargetInstant", _targetInstant?.GetStorageString() ?? MudInstant.Never.GetStorageString()))
			.ToString();
	}

	public override void Login()
	{
		_runtimeActive = true;
		base.Login();
		if (!Armed)
		{
			return;
		}

		EnsureClockSubscription();
		CheckTargetTime();
	}

	public override void Quit()
	{
		_runtimeActive = false;
		RemoveClockSubscription();
		base.Quit();
	}

	public override void Delete()
	{
		_runtimeActive = false;
		RemoveClockSubscription();
		base.Delete();
	}

	public bool CanArm(ICharacter actor, string argument)
	{
		return !Armed && Parent.IsItemType<IDetonatable>() && TryParseTarget(actor, argument, out _, out _);
	}

	public string WhyCannotArm(ICharacter actor, string argument)
	{
		if (Armed)
		{
			return $"{Parent.HowSeen(actor, true)} is already armed for {DescribeTarget(actor)}.";
		}

		if (!Parent.IsItemType<IDetonatable>())
		{
			return $"{Parent.HowSeen(actor, true)} has no explosive payload to detonate.";
		}

		return TryParseTarget(actor, argument, out _, out var error)
			? $"{Parent.HowSeen(actor, true)} cannot be armed at this time."
			: error;
	}

	public bool Arm(ICharacter actor, string argument, IEmote? playerEmote = null)
	{
		if (!CanArm(actor, argument))
		{
			actor.Send(WhyCannotArm(actor, argument));
			return false;
		}

		TryParseTarget(actor, argument, out var target, out _);
		_targetInstant = target;
		Changed = true;
		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ArmEmote, actor, actor, Parent)).Append(playerEmote));
		if (_runtimeActive)
		{
			EnsureClockSubscription();
			CheckTargetTime();
		}
		return true;
	}

	public bool CanDisarm(ICharacter actor)
	{
		return Armed && _prototype.CanBeDisarmed;
	}

	public string WhyCannotDisarm(ICharacter actor)
	{
		if (!Armed)
		{
			return $"{Parent.HowSeen(actor, true)} is not armed.";
		}

		return _prototype.CanBeDisarmed
			? $"{Parent.HowSeen(actor, true)} cannot be disarmed at this time."
			: $"Once armed, {Parent.HowSeen(actor)} has an irreversible clock trigger.";
	}

	public bool Disarm(ICharacter actor, IEmote? playerEmote = null)
	{
		if (!CanDisarm(actor))
		{
			actor.Send(WhyCannotDisarm(actor));
			return false;
		}

		actor.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.DisarmEmote, actor, actor, Parent)).Append(playerEmote));
		_targetInstant = null;
		Changed = true;
		RemoveClockSubscription();
		return true;
	}

	private bool TryParseTarget(ICharacter actor, string argument, out MudInstant instant, out string error)
	{
		instant = MudInstant.Never;
		error = string.Empty;
		if (string.IsNullOrWhiteSpace(argument))
		{
			error = "You must specify the exact in-game date and time at which this detonator should fire.";
			return false;
		}

		var localTargetText = $"{argument} {_prototype.TimeZone.Alias}";
		if (!MudDateTime.TryParse(localTargetText, _prototype.Calendar, _prototype.Clock, actor, out var dateTime,
			    out error))
		{
			return false;
		}

		instant = MudInstant.FromMudDateTime(dateTime);
		if (instant.IsNever || instant <= _prototype.Calendar.CurrentInstant)
		{
			error = "The detonation time must be in the future.";
			return false;
		}

		return true;
	}

	private void EnsureClockSubscription()
	{
		if (_clockSubscribed || !Armed)
		{
			return;
		}

		_prototype.Clock.SecondsUpdated += CheckTargetTime;
		_clockSubscribed = true;
	}

	private void RemoveClockSubscription()
	{
		if (!_clockSubscribed)
		{
			return;
		}

		_prototype.Clock.SecondsUpdated -= CheckTargetTime;
		_clockSubscribed = false;
	}

	private void CheckTargetTime()
	{
		if (_targetInstant is not { } target ||
		    !ClockDetonatorScheduleEvaluator.IsDue(target, _prototype.Calendar.CurrentInstant))
		{
			return;
		}

		_targetInstant = null;
		Changed = true;
		RemoveClockSubscription();
		Parent.GetItemType<IDetonatable>()?.Detonate();
	}

	private string DescribeTarget(IFormatProvider voyeur)
	{
		if (_targetInstant is not { } target)
		{
			return "no target time";
		}

		var dateTime = target.ToMudDateTime(_prototype.Calendar, _prototype.Clock, _prototype.TimeZone);
		return dateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue();
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short && Armed)
		{
			return $"{description} {"(armed)".Colour(Telnet.BoldRed)}";
		}

		if (type == DescriptionType.Full)
		{
			return Armed
				? $"{description}\n\nIts clock detonator is armed for {DescribeTarget(voyeur)}."
				: $"{description}\n\nIts clock detonator is currently disarmed.";
		}

		if (type == DescriptionType.Evaluate)
		{
			return
				$"It has a clock detonator using {_prototype.Calendar.FullName.ColourName()}, {_prototype.Clock.Name.ColourName()}, and {_prototype.TimeZone.Alias.ColourValue()}. It {(_prototype.CanBeDisarmed ? "can" : "cannot")} be disarmed after arming.";
		}

		return description;
	}

	public override int DecorationPriority => int.MaxValue - 1;
}
