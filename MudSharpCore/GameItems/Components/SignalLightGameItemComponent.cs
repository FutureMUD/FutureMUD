#nullable enable

using MudSharp.Computers;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class SignalLightGameItemComponent : ProgLightGameItemComponent, ISignalSinkComponent
{
	private SignalLightGameItemComponentProto _signalPrototype;
	private readonly LocalSignalSinkSubscription _binding;

	public SignalLightGameItemComponent(SignalLightGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_signalPrototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public SignalLightGameItemComponent(MudSharp.Models.GameItemComponent component,
		SignalLightGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_signalPrototype = proto;
		_binding = new LocalSignalSinkSubscription(parent, this, HandleSourceChanged);
	}

	public SignalLightGameItemComponent(SignalLightGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_signalPrototype = rhs._signalPrototype;
		_binding = new LocalSignalSinkSubscription(newParent, this, HandleSourceChanged);
	}

	public long SourceComponentId => _signalPrototype.SourceComponentId;
	public string SourceComponentName => _signalPrototype.SourceComponentName;
	public string SourceEndpointKey => _signalPrototype.SourceEndpointKey;
	public ISignalSource? UpstreamSource => _binding.UpstreamSource;
	public double CurrentValue { get; private set; }

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SignalLightGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		base.UpdateComponentNewPrototype(newProto);
		_signalPrototype = (SignalLightGameItemComponentProto)newProto;
	}

	public override void FinaliseLoad()
	{
		ReconnectSource();
	}

	public override void Delete()
	{
		_binding.Detach();
		base.Delete();
	}

	public override void Quit()
	{
		_binding.Detach();
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
		CurrentValue = signal.Value;
		ApplySignalValue(signal.Value);
	}

	private void ApplySignalValue(double value)
	{
		CurrentValue = value;
		var desiredLit = SignalComponentUtilities.IsActiveSignal(value, _signalPrototype.ActivationThreshold,
			_signalPrototype.LitWhenAboveThreshold);
		if (desiredLit == Lit)
		{
			return;
		}

		Lit = desiredLit;
		var emoteText = desiredLit ? _signalPrototype.LightOnEmote : _signalPrototype.LightOffEmote;
		if (!string.IsNullOrWhiteSpace(emoteText))
		{
			Parent.Handle(
				new EmoteOutput(new Emote(emoteText, Parent, Parent), flags: OutputFlags.SuppressObscured),
				OutputRange.Local);
		}
	}

	private void HandleSourceChanged(ISignalSourceComponent source, ComputerSignal signal)
	{
		ReceiveSignal(signal, source);
	}
}
