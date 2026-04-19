#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Components;

public class TemperatureSensorGameItemComponent : PoweredMachineBaseGameItemComponent, ISignalSourceComponent
{
	private TemperatureSensorGameItemComponentProto _prototype;
	private bool _heartbeatSubscribed;
	private ComputerSignal _currentSignal;
	private double _currentTemperatureCelsius;

	public TemperatureSensorGameItemComponent(TemperatureSensorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public TemperatureSensorGameItemComponent(MudSharp.Models.GameItemComponent component,
		TemperatureSensorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public TemperatureSensorGameItemComponent(TemperatureSensorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_currentSignal = rhs._currentSignal;
		_currentTemperatureCelsius = rhs._currentTemperatureCelsius;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long LocalSignalSourceIdentifier => Prototype.Id;
	public string EndpointKey => SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public ComputerSignal CurrentSignal => _currentSignal;
	public event SignalChangedEvent? SignalChanged;
	public double CurrentValue => _currentSignal.Value;
	public TimeSpan? Duration => null;
	public TimeSpan? PulseInterval => null;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TemperatureSensorGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return
			$"{description}\n\nIts temperature sensor is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(_onAndPowered ? "powered".ColourValue() : "not powered".ColourError())}, currently reading {_currentTemperatureCelsius.ToString("N2", voyeur).ColourValue()} degrees Celsius and outputting {_currentSignal.Value.ToString("N2", voyeur).ColourValue()} on signal.";
	}

	public override int DecorationPriority => 1000;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (TemperatureSensorGameItemComponentProto)newProto;
		RefreshSignalState(true);
	}

	public override void FinaliseLoad()
	{
		RefreshSignalState(false);
	}

	public override void Login()
	{
		base.Login();
		EnsureHeartbeatSubscription();
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

	private void HeartbeatTick()
	{
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
		_currentTemperatureCelsius = ResolveCurrentTemperatureCelsius();
		var desiredSignal = SwitchedOn && _onAndPowered
			? new ComputerSignal(_currentTemperatureCelsius, null, null)
			: default;
		SetCurrentSignal(desiredSignal, markChanged);
		HandleDescriptionUpdate();
	}

	private double ResolveCurrentTemperatureCelsius()
	{
		var anchorItem = SignalComponentUtilities.ResolveSignalSearchAnchorItem(Parent);
		var cell = anchorItem.TrueLocations
			           .OfType<ICell>()
			           .FirstOrDefault() ??
		           Parent.TrueLocations
			           .OfType<ICell>()
			           .FirstOrDefault();
		if (cell is null)
		{
			return 0.0;
		}

		return cell.CurrentTemperature(anchorItem) * Gameworld.UnitManager.BaseTemperatureToCelcius;
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
