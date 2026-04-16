#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ElectronicDoorGameItemComponent : DoorGameItemComponentBase, ISignalSinkComponent
{
	private ElectronicDoorGameItemComponentProto _prototype;
	private readonly LocalSignalSinkSubscription _binding;
	private bool _desiredOpen;
	private bool _heartbeatSubscribed;

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
	}

	public ElectronicDoorGameItemComponent(ElectronicDoorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
		_desiredOpen = rhs._desiredOpen;
		CurrentValue = rhs.CurrentValue;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long SourceComponentId => _prototype.SourceComponentId;
	public string SourceComponentName => _prototype.SourceComponentName;
	public string SourceEndpointKey => _prototype.SourceEndpointKey;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ElectronicDoorGameItemComponent(this, newParent, temporary);
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		var baseDescription = base.Decorate(voyeur, name, description, type, colour, flags);
		return
			$"{baseDescription}\n\nIts electronic controller is listening to {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()} with a current control signal of {CurrentValue.ToString("N2", voyeur).ColourValue()}, and is presently commanding the door to {(_desiredOpen ? "open".ColourValue() : "remain closed".ColourName())}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_prototype = (ElectronicDoorGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		base.FinaliseLoad();
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
		_binding.Reconnect(SourceComponentId, SourceComponentName, SourceEndpointKey);
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
		_desiredOpen = SignalComponentUtilities.IsActiveSignal(value, _prototype.ActivationThreshold,
			_prototype.OpenWhenAboveThreshold);
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
}
