#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

internal readonly record struct TimerSensorCycleState(bool IsActive, DateTime NextTransition);

internal static class TimerSensorCycleScheduler
{
	internal static TimerSensorCycleState Resolve(DateTime cycleAnchor, bool startsActive, TimeSpan activeDuration,
		TimeSpan inactiveDuration, DateTime now)
	{
		if (activeDuration <= TimeSpan.Zero || inactiveDuration <= TimeSpan.Zero)
		{
			return new TimerSensorCycleState(startsActive, DateTime.MaxValue);
		}

		var anchor = cycleAnchor.Kind == DateTimeKind.Utc
			? cycleAnchor
			: DateTime.SpecifyKind(cycleAnchor, DateTimeKind.Utc);
		if (now <= anchor)
		{
			return new TimerSensorCycleState(startsActive, anchor + (startsActive ? activeDuration : inactiveDuration));
		}

		var activeTicks = activeDuration.Ticks;
		var inactiveTicks = inactiveDuration.Ticks;
		var cycleTicks = activeTicks + inactiveTicks;
		if (cycleTicks <= 0)
		{
			return new TimerSensorCycleState(startsActive, DateTime.MaxValue);
		}

		var elapsedTicks = now.Ticks - anchor.Ticks;
		var cycleIndex = elapsedTicks / cycleTicks;
		var offsetTicks = elapsedTicks % cycleTicks;
		if (startsActive)
		{
			if (offsetTicks < activeTicks)
			{
				return new TimerSensorCycleState(true, anchor.AddTicks(cycleIndex * cycleTicks + activeTicks));
			}

			return new TimerSensorCycleState(false, anchor.AddTicks((cycleIndex + 1L) * cycleTicks));
		}

		if (offsetTicks < inactiveTicks)
		{
			return new TimerSensorCycleState(false, anchor.AddTicks(cycleIndex * cycleTicks + inactiveTicks));
		}

		return new TimerSensorCycleState(true, anchor.AddTicks((cycleIndex + 1L) * cycleTicks));
	}
}

public class TimerSensorGameItemComponent : PoweredMachineBaseGameItemComponent, ISignalSourceComponent
{
	private TimerSensorGameItemComponentProto _prototype;
	private DateTime _cycleAnchor;
	private DateTime _nextTransition;
	private bool _startsActive;
	private bool _isActive;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;

	public TimerSensorGameItemComponent(TimerSensorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		var now = DateTime.UtcNow;
		_cycleAnchor = now;
		_startsActive = proto.StartActive;
		RefreshState(now, false);
		if (!temporary)
		{
			EnsureHeartbeatSubscription();
		}
	}

	public TimerSensorGameItemComponent(MudSharp.Models.GameItemComponent component,
		TimerSensorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TimerSensorGameItemComponent(TimerSensorGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_cycleAnchor = rhs._cycleAnchor;
		_nextTransition = rhs._nextTransition;
		_startsActive = rhs._startsActive;
		_isActive = rhs._isActive;
		_currentSignal = rhs._currentSignal;
		if (!temporary)
		{
			EnsureHeartbeatSubscription();
		}
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TimerSensorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var untilTransition = _nextTransition > DateTime.UtcNow
			? (_nextTransition - DateTime.UtcNow).Describe(voyeur).ColourValue()
			: "less than a second".ColourValue();
		return
			$"{description}\n\nIts timer sensor is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(_onAndPowered ? "powered".ColourValue() : "not powered".ColourError())}, alternates between {_prototype.ActiveValue.ToString("N2", voyeur).ColourValue()} for {_prototype.ActiveDuration.Describe(voyeur).ColourValue()} and {_prototype.InactiveValue.ToString("N2", voyeur).ColourValue()} for {_prototype.InactiveDuration.Describe(voyeur).ColourValue()}. It is currently {(_isActive ? "active".ColourValue() : "inactive".ColourName())} and will change state in {untilTransition}.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (TimerSensorGameItemComponentProto)newProto;
		RefreshState(DateTime.UtcNow, true);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		var cycleAnchorElement = root.Element("CycleAnchorTicks");
		if (cycleAnchorElement is not null && long.TryParse(cycleAnchorElement.Value, out var cycleAnchorTicks) &&
		    cycleAnchorTicks > 0)
		{
			_cycleAnchor = new DateTime(cycleAnchorTicks, DateTimeKind.Utc);
		}
		else
		{
			_cycleAnchor = DateTime.UtcNow;
		}

		var startsActiveElement = root.Element("StartsActive");
		_startsActive = startsActiveElement is not null
			? bool.Parse(startsActiveElement.Value)
			: _prototype.StartActive;
	}

	public override void FinaliseLoad()
	{
		var now = DateTime.UtcNow;
		if (_cycleAnchor == default)
		{
			_cycleAnchor = now;
			_startsActive = _prototype.StartActive;
		}

		RefreshState(now, false);
		EnsureHeartbeatSubscription();
	}

	public override void Delete()
	{
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	public override void Quit()
	{
		RemoveHeartbeatSubscription();
		base.Quit();
	}

	protected override XElement SaveToXml(XElement root)
	{
		root.Add(new XElement("CycleAnchorTicks", _cycleAnchor.Ticks));
		root.Add(new XElement("StartsActive", _startsActive));
		return root;
	}

	protected override void OnPowerCutInAction()
	{
		RefreshState(DateTime.UtcNow, true);
	}

	protected override void OnPowerCutOutAction()
	{
		SetCurrentSignal(default, true);
		HandleDescriptionUpdate();
	}

	private void HeartbeatTick()
	{
		RefreshState(DateTime.UtcNow, true);
	}

	private void EnsureHeartbeatSubscription()
	{
		if (_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatTick;
		_heartbeatSubscribed = true;
	}

	private void RemoveHeartbeatSubscription()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatTick;
		_heartbeatSubscribed = false;
	}

	private void RefreshState(DateTime now, bool markChanged)
	{
		var previousIsActive = _isActive;
		var state = TimerSensorCycleScheduler.Resolve(_cycleAnchor, _startsActive, _prototype.ActiveDuration,
			_prototype.InactiveDuration, now);
		_isActive = state.IsActive;
		_nextTransition = state.NextTransition;
		var desiredSignal = SwitchedOn && _onAndPowered
			? (_isActive
				? new ComputerSignal(_prototype.ActiveValue, _prototype.ActiveDuration, null)
				: new ComputerSignal(_prototype.InactiveValue, _prototype.InactiveDuration, null))
			: default;
		SetCurrentSignal(desiredSignal, markChanged);
		if (previousIsActive != _isActive)
		{
			if (markChanged)
			{
				Changed = true;
			}

			HandleDescriptionUpdate();
		}
	}

	private void SetCurrentSignal(ComputerSignal signal, bool markChanged)
	{
		if (SignalComponentUtilities.SignalsEqual(_currentSignal, signal))
		{
			return;
		}

		_currentSignal = signal;
		if (markChanged)
		{
			Changed = true;
		}

		SignalChanged?.Invoke(this, _currentSignal);
	}
}
