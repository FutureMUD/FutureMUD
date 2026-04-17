#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class MotionSensorGameItemComponent : PoweredMachineBaseGameItemComponent, ISignalSourceComponent
{
	private MotionSensorGameItemComponentProto _prototype;
	private DateTime? _activeUntil;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;

	public MotionSensorGameItemComponent(MotionSensorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public MotionSensorGameItemComponent(MudSharp.Models.GameItemComponent component,
		MotionSensorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
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
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
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
			$"{description}\n\nIts motion sensor is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(_onAndPowered ? "powered".ColourValue() : "not powered".ColourError())}, watching for {_prototype.DetectionMode.Describe().ColourValue()} from {_prototype.MinimumSize.Describe().ColourValue()} targets and is currently {(_activeUntil is not null && _onAndPowered ? "active".ColourValue() : "inactive".ColourName())}.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (MotionSensorGameItemComponentProto)newProto;
		RefreshSignalState(false);
	}

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		var element = root.Element("ActiveUntilTicks");
		if (element is not null && long.TryParse(element.Value, out var ticks) && ticks > 0)
		{
			_activeUntil = new DateTime(ticks, DateTimeKind.Utc);
		}
	}

	public override void FinaliseLoad()
	{
		if (_activeUntil is not null && _activeUntil > DateTime.UtcNow)
		{
			EnsureHeartbeatSubscription();
		}
		else
		{
			_activeUntil = null;
		}

		RefreshSignalState(false);
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
		root.Add(new XElement("ActiveUntilTicks", _activeUntil?.Ticks ?? 0));
		return root;
	}

	protected override void OnPowerCutInAction()
	{
		RefreshSignalState(true);
	}

	protected override void OnPowerCutOutAction()
	{
		SetCurrentSignal(default, true);
		HandleDescriptionUpdate();
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (!SwitchedOn || !_onAndPowered || !_prototype.DetectionMode.MatchesEventType(type) || arguments.Length == 0 ||
		    arguments[0] is not ICharacter mover || mover.Size < _prototype.MinimumSize)
		{
			return false;
		}

		_activeUntil = DateTime.UtcNow + _prototype.SignalDuration;
		Changed = true;
		EnsureHeartbeatSubscription();
		RefreshSignalState(false);
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
		RefreshSignalState(true);
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

	private void RefreshSignalState(bool markChanged)
	{
		var shouldBeActive = _activeUntil is not null &&
		                     _activeUntil > DateTime.UtcNow &&
		                     SwitchedOn &&
		                     _onAndPowered;
		SetCurrentSignal(shouldBeActive
			? new ComputerSignal(_prototype.SignalValue, _prototype.SignalDuration, null)
			: default, markChanged);
		HandleDescriptionUpdate();
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
