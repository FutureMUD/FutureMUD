#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class ElectronicDoorGameItemComponent : DoorGameItemComponentBase, IRuntimeConfigurableSignalSinkComponent
{
	private ElectronicDoorGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;
	private bool _desiredOpen;
	private bool _heartbeatSubscribed;
	private LocalSignalBinding? _runtimeBinding;
	private double? _runtimeActivationThreshold;
	private bool? _runtimeActiveWhenAboveThreshold;

	public ElectronicDoorGameItemComponent(ElectronicDoorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public ElectronicDoorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectronicDoorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
		LoadRuntimeConfiguration(XElement.Parse(component.Definition));
	}

	public ElectronicDoorGameItemComponent(ElectronicDoorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
		_desiredOpen = rhs._desiredOpen;
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
	public bool ActiveWhenAboveThreshold => _runtimeActiveWhenAboveThreshold ?? _prototype.OpenWhenAboveThreshold;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectronicDoorGameItemComponent(this, newParent, temporary);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ElectronicDoorGameItemComponentProto)newProto;
	}

	protected override void SaveAdditionalToXml(XElement root)
	{
		base.SaveAdditionalToXml(root);
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
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
	}

	public override void Login()
	{
		base.Login();
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

	public void ReconnectSource()
	{
		_binding.Reconnect(CurrentBinding);
		if (_binding.UpstreamSource is not null)
		{
			RemoveHeartbeatSubscription();
			return;
		}

		ApplySignalValue(0.0);
		EnsureHeartbeatSubscription();
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		ApplySignalValue(signal.Value);
	}

	private void ApplySignalValue(double value)
	{
		CurrentValue = value;
		_desiredOpen = SignalComponentUtilities.IsActiveSignal(value, ActivationThreshold,
			ActiveWhenAboveThreshold);
		EvaluateDoorState();
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}

	private void EvaluateDoorState()
	{
		var previousState = IsOpen;
		var outcome = ElectronicDoorControlEvaluator.Evaluate(_desiredOpen, IsOpen, CanOpen(null), CanClose(null));
		switch (outcome.Action)
		{
			case ElectronicDoorControlAction.Open:
				Open();
				break;
			case ElectronicDoorControlAction.Close:
				Close();
				break;
		}

		if (outcome.RequiresRetry)
		{
			EnsureHeartbeatSubscription();
		}
		else
		{
			RemoveHeartbeatSubscription();
		}

		if (previousState == IsOpen)
		{
			return;
		}

		HandleDescriptionUpdate();
		var emoteText = IsOpen ? _prototype.OpenEmoteNoActor : _prototype.CloseEmoteNoActor;
		if (string.IsNullOrWhiteSpace(emoteText))
		{
			return;
		}

		Parent.Handle(
			new EmoteOutput(new Emote(emoteText, Parent, Parent), flags: OutputFlags.SuppressObscured),
			OutputRange.Local);
	}

	private void HeartbeatTick()
	{
		if (_binding.UpstreamSource is null)
		{
			ReconnectSource();
			return;
		}

		EvaluateDoorState();
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

	private void LoadRuntimeConfiguration(XElement root)
	{
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
}
