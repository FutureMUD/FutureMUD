#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class AlarmSirenGameItemComponent : PoweredMachineBaseGameItemComponent, ISignalSinkComponent
{
	private AlarmSirenGameItemComponentProto _prototype;
	private ISignalSourceComponent? _source;
	private bool _heartbeatSubscribed;
	private bool _signalActive;

	public AlarmSirenGameItemComponent(AlarmSirenGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public AlarmSirenGameItemComponent(MudSharp.Models.GameItemComponent component, AlarmSirenGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public AlarmSirenGameItemComponent(AlarmSirenGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_signalActive = rhs._signalActive;
		CurrentValue = rhs.CurrentValue;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string SourceComponentName => _prototype.SourceComponentName;
	public ISignalSource? UpstreamSource => _source;
	public double CurrentValue { get; private set; }
	private bool IsSounding => _signalActive && SwitchedOn && _onAndPowered;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AlarmSirenGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full;
	}

	public override int DecorationPriority => 1000;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Short => $"{description}{(IsSounding ? " (sounding)".FluentColour(Telnet.Red, colour) : string.Empty)}",
			DescriptionType.Full =>
				$"{description}\n\nIts alarm siren is {(SwitchedOn ? "switched on".ColourValue() : "switched off".ColourError())}, {(_onAndPowered ? "powered".ColourValue() : "not powered".ColourError())}, and {(IsSounding ? "currently sounding".ColourError() : "currently silent".ColourName())}.",
			_ => description
		};
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (AlarmSirenGameItemComponentProto)newProto;
	}

	protected override XElement SaveToXml(XElement root)
	{
		return root;
	}

	public override void FinaliseLoad()
	{
		ReconnectSource();
	}

	public override void Delete()
	{
		DetachSource();
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	public override void Quit()
	{
		DetachSource();
		RemoveHeartbeatSubscription();
		base.Quit();
	}

	protected override void OnPowerCutInAction()
	{
		EvaluateAlarmState();
	}

	protected override void OnPowerCutOutAction()
	{
		EvaluateAlarmState();
	}

	public void ReconnectSource()
	{
		DetachSource();
		_source = SignalComponentUtilities.FindSignalSource(Parent, SourceComponentName, this);
		if (_source is null)
		{
			CurrentValue = 0.0;
			_signalActive = false;
			EvaluateAlarmState();
			return;
		}

		_source.SignalChanged += HandleSourceChanged;
		ReceiveSignal(_source.CurrentSignal, _source);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		CurrentValue = signal.Value;
		_signalActive = SignalComponentUtilities.IsActiveSignal(signal.Value, _prototype.ActivationThreshold,
			_prototype.SoundWhenAboveThreshold);
		EvaluateAlarmState();
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}

	private void DetachSource()
	{
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged -= HandleSourceChanged;
		_source = null;
	}

	private void AlarmHeartbeat()
	{
		if (!IsSounding)
		{
			RemoveHeartbeatSubscription();
			return;
		}

		EmitAlarm();
	}

	private void EvaluateAlarmState()
	{
		var shouldSound = IsSounding;
		var wasSounding = _heartbeatSubscribed;
		if (shouldSound)
		{
			if (!_heartbeatSubscribed)
			{
				EmitAlarm();
				Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat += AlarmHeartbeat;
				_heartbeatSubscribed = true;
			}
		}
		else
		{
			RemoveHeartbeatSubscription();
		}

		if (wasSounding != _heartbeatSubscribed)
		{
			HandleDescriptionUpdate();
		}
	}

	private void RemoveHeartbeatSubscription()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.FuzzyFiveSecondHeartbeat -= AlarmHeartbeat;
		_heartbeatSubscribed = false;
	}

	private void EmitAlarm()
	{
		Parent.Handle(
			new AudioOutput(new Emote(_prototype.AlarmEmote, Parent, Parent), _prototype.AlarmVolume,
				flags: OutputFlags.PurelyAudible),
			OutputRange.Local);
		foreach (var location in Parent.TrueLocations.Distinct())
		{
			location.HandleAudioEcho("You hear an alarm siren {0}.", _prototype.AlarmVolume, Parent, Parent.RoomLayer);
		}
	}
}
