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
	private ISignalSourceComponent? _source;

	public SignalLightGameItemComponent(SignalLightGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(proto, parent, temporary)
	{
		_signalPrototype = proto;
	}

	public SignalLightGameItemComponent(MudSharp.Models.GameItemComponent component,
		SignalLightGameItemComponentProto proto, IGameItem parent)
		: base(component, proto, parent)
	{
		_signalPrototype = proto;
	}

	public SignalLightGameItemComponent(SignalLightGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_signalPrototype = rhs._signalPrototype;
	}

	public string SourceComponentName => _signalPrototype.SourceComponentName;
	public ISignalSource? UpstreamSource => _source;
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
		DetachSource();
		base.Delete();
	}

	public override void Quit()
	{
		DetachSource();
		base.Quit();
	}

	public void ReconnectSource()
	{
		DetachSource();
		_source = SignalComponentUtilities.FindSignalSource(Parent, SourceComponentName, this);
		if (_source is null)
		{
			return;
		}

		_source.SignalChanged += HandleSourceChanged;
		ReceiveSignal(_source.CurrentSignal, _source);
	}

	public void ReceiveSignal(ComputerSignal signal, ISignalSource source)
	{
		CurrentValue = signal.Value;
		var desiredLit =
			SignalComponentUtilities.IsActiveSignal(signal.Value, _signalPrototype.ActivationThreshold,
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
