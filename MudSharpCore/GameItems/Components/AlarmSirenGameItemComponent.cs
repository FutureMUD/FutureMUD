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

public class AlarmSirenGameItemComponent : PoweredMachineBaseGameItemComponent, IRuntimeConfigurableSignalSinkComponent
{
	private AlarmSirenGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;
	private bool _heartbeatSubscribed;
	private bool _signalActive;
	private LocalSignalBinding? _runtimeBinding;
	private double? _runtimeActivationThreshold;
	private bool? _runtimeActiveWhenAboveThreshold;

	public AlarmSirenGameItemComponent(AlarmSirenGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public AlarmSirenGameItemComponent(MudSharp.Models.GameItemComponent component, AlarmSirenGameItemComponentProto proto,
		IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public AlarmSirenGameItemComponent(AlarmSirenGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
		_signalActive = rhs._signalActive;
		CurrentValue = rhs.CurrentValue;
		_runtimeBinding = rhs._runtimeBinding;
		_runtimeActivationThreshold = rhs._runtimeActivationThreshold;
		_runtimeActiveWhenAboveThreshold = rhs._runtimeActiveWhenAboveThreshold;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long SourceComponentId => CurrentBinding.SourceComponentId;
	public string SourceComponentName => CurrentBinding.SourceComponentName;
	public string SourceEndpointKey => CurrentBinding.SourceEndpointKey;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }
	public LocalSignalBinding CurrentBinding => _runtimeBinding ?? new LocalSignalBinding(
		0L,
		string.Empty,
		_prototype.SourceComponentId,
		_prototype.SourceComponentName,
		_prototype.SourceEndpointKey);
	public double ActivationThreshold => _runtimeActivationThreshold ?? _prototype.ActivationThreshold;
	public bool ActiveWhenAboveThreshold => _runtimeActiveWhenAboveThreshold ?? _prototype.SoundWhenAboveThreshold;
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

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		var runtimeSourceId = root.Element("RuntimeSourceComponentId");
		if (runtimeSourceId is not null)
		{
			_runtimeBinding = new LocalSignalBinding(
				long.TryParse(root.Element("RuntimeSourceItemId")?.Value, out var sourceItemId) ? sourceItemId : 0L,
				root.Element("RuntimeSourceItemName")?.Value ?? string.Empty,
				long.TryParse(runtimeSourceId.Value, out var sourceId) ? sourceId : 0L,
				root.Element("RuntimeSourceComponentName")?.Value ?? string.Empty,
				SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("RuntimeSourceEndpointKey")?.Value));
		}

		if (double.TryParse(root.Element("RuntimeActivationThreshold")?.Value, out var activationThreshold))
		{
			_runtimeActivationThreshold = activationThreshold;
		}

		if (bool.TryParse(root.Element("RuntimeActiveWhenAboveThreshold")?.Value, out var activeWhenAboveThreshold))
		{
			_runtimeActiveWhenAboveThreshold = activeWhenAboveThreshold;
		}
	}

	protected override XElement SaveToXml(XElement root)
	{
		if (_runtimeBinding is not null)
		{
			root.Add(new XElement("RuntimeSourceItemId", _runtimeBinding.SourceItemId));
			root.Add(new XElement("RuntimeSourceItemName", new XCData(_runtimeBinding.SourceItemName)));
			root.Add(new XElement("RuntimeSourceComponentId", _runtimeBinding.SourceComponentId));
			root.Add(new XElement("RuntimeSourceComponentName", new XCData(_runtimeBinding.SourceComponentName)));
			root.Add(new XElement("RuntimeSourceEndpointKey", new XCData(_runtimeBinding.SourceEndpointKey)));
		}

		if (_runtimeActivationThreshold.HasValue)
		{
			root.Add(new XElement("RuntimeActivationThreshold", _runtimeActivationThreshold.Value));
		}

		if (_runtimeActiveWhenAboveThreshold.HasValue)
		{
			root.Add(new XElement("RuntimeActiveWhenAboveThreshold", _runtimeActiveWhenAboveThreshold.Value));
		}

		return root;
	}

	public override void FinaliseLoad()
	{
		ReconnectSource();
	}

	public override void Delete()
	{
		_binding.Detach();
		RemoveHeartbeatSubscription();
		base.Delete();
	}

	public override void Quit()
	{
		_binding.Detach();
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
		_binding.Reconnect(CurrentBinding);
		if (_binding.UpstreamSource is not null)
		{
			return;
		}

		ApplySignalValue(0.0);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		ApplySignalValue(signal.Value);
	}

	private void ApplySignalValue(double value)
	{
		CurrentValue = value;
		_signalActive = SignalComponentUtilities.IsActiveSignal(value, ActivationThreshold,
			ActiveWhenAboveThreshold);
		EvaluateAlarmState();
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
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

	public bool ConfigureSignalBinding(ISignalSourceComponent source, string? endpointKey, out string error)
	{
		_runtimeBinding = SignalComponentUtilities.CreateBinding(source, endpointKey);
		Changed = true;
		ReconnectSource();
		error = string.Empty;
		return true;
	}

	public void ClearSignalBinding()
	{
		_runtimeBinding = null;
		Changed = true;
		ReconnectSource();
	}

	public bool SetActivationThreshold(double threshold, out string error)
	{
		if (double.IsNaN(threshold) || double.IsInfinity(threshold))
		{
			error = "That is not a valid numeric threshold.";
			return false;
		}

		_runtimeActivationThreshold = threshold;
		Changed = true;
		ApplySignalValue(CurrentValue);
		error = string.Empty;
		return true;
	}

	public void SetActiveWhenAboveThreshold(bool activeWhenAboveThreshold)
	{
		_runtimeActiveWhenAboveThreshold = activeWhenAboveThreshold;
		Changed = true;
		ApplySignalValue(CurrentValue);
	}
}
