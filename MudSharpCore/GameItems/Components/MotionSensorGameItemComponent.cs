#nullable enable

using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class MotionSensorGameItemComponent : GameItemComponent, ISignalSourceComponent
{
	private MotionSensorGameItemComponentProto _prototype;
	private DateTime? _activeUntil;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;

	public MotionSensorGameItemComponent(MotionSensorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public MotionSensorGameItemComponent(MudSharp.Models.GameItemComponent component,
		MotionSensorGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public MotionSensorGameItemComponent(MotionSensorGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_activeUntil = rhs._activeUntil;
		_currentSignal = rhs._currentSignal;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => _currentSignal.Duration;
	public TimeSpan? PulseInterval => _currentSignal.PulseInterval;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new MotionSensorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return
			$"{description}\n\nIts motion sensor is watching for {_prototype.DetectionMode.Describe().ColourValue()} from {_prototype.MinimumSize.Describe().ColourValue()} targets and is currently {(_activeUntil is not null ? "active".ColourValue() : "inactive".ColourName())}.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (MotionSensorGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		if (_activeUntil is not null && _activeUntil > DateTime.UtcNow)
		{
			_currentSignal = new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null);
			EnsureHeartbeatSubscription();
			return;
		}

		_activeUntil = null;
		_currentSignal = default;
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

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("ActiveUntilTicks", _activeUntil?.Ticks ?? 0)
		).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		var element = root.Element("ActiveUntilTicks");
		if (element is not null && long.TryParse(element.Value, out var ticks) && ticks > 0)
		{
			_activeUntil = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (!_prototype.DetectionMode.MatchesEventType(type) || arguments.Length == 0 ||
		    arguments[0] is not ICharacter mover || mover.Size < _prototype.MinimumSize)
		{
			return false;
		}

		_activeUntil = DateTime.UtcNow + _prototype.SignalDuration;
		Changed = true;
		EnsureHeartbeatSubscription();
		SetCurrentSignal(new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null), false);
		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Any(x => _prototype.DetectionMode.MatchesEventType(x));
	}

	private void HeartbeatTick()
	{
		if (_activeUntil is null || _activeUntil > DateTime.UtcNow)
		{
			return;
		}

		_activeUntil = null;
		RemoveHeartbeatSubscription();
		SetCurrentSignal(default, true);
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
