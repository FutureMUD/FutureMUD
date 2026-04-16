#nullable enable

using MudSharp.Computers;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ElectronicDoorGameItemComponent : DoorGameItemComponent, ISignalSinkComponent
{
	private ElectronicDoorGameItemComponentProto _prototype;
	private ISignalSourceComponent? _source;
	private bool _desiredOpen;
	private bool _heartbeatSubscribed;

	public ElectronicDoorGameItemComponent(ElectronicDoorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public ElectronicDoorGameItemComponent(MudSharp.Models.GameItemComponent component,
		ElectronicDoorGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_prototype = proto;
	}

	public ElectronicDoorGameItemComponent(ElectronicDoorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_desiredOpen = rhs._desiredOpen;
		CurrentValue = rhs.CurrentValue;
	}

	public string SourceComponentName => _prototype.SourceComponentName;
	public ISignalSource? UpstreamSource => _source;
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
			$"{baseDescription}\n\nIts electronic controller is listening to {SourceComponentName.ColourName()} with a current control signal of {CurrentValue.ToString("N2", voyeur).ColourValue()}, and is presently commanding the door to {(_desiredOpen ? "open".ColourValue() : "remain closed".ColourName())}.";
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

	public void ReconnectSource()
	{
		DetachSource();
		_source = SignalComponentUtilities.FindSignalSource(Parent, SourceComponentName, this);
		if (_source is null)
		{
			CurrentValue = 0.0;
			_desiredOpen = SignalComponentUtilities.IsActiveSignal(0.0, _prototype.ActivationThreshold,
				_prototype.OpenWhenAboveThreshold);
			EvaluateDoorState();
			return;
		}

		_source.SignalChanged += HandleSourceChanged;
		ReceiveSignal(_source.CurrentSignal, _source);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		CurrentValue = signal.Value;
		_desiredOpen = SignalComponentUtilities.IsActiveSignal(signal.Value, _prototype.ActivationThreshold,
			_prototype.OpenWhenAboveThreshold);
		EvaluateDoorState();
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}

	private void EvaluateDoorState()
	{
		var wasOpen = IsOpen;
		if (_desiredOpen)
		{
			if (!IsOpen && CanOpen(null))
			{
				Open();
			}
		}
		else
		{
			if (IsOpen && CanClose(null))
			{
				Close();
			}
		}

		if (_desiredOpen != IsOpen)
		{
			EnsureHeartbeatSubscription();
		}
		else
		{
			RemoveHeartbeatSubscription();
		}

		if (wasOpen == IsOpen)
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

	private void DetachSource()
	{
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged -= HandleSourceChanged;
		_source = null;
	}
}
