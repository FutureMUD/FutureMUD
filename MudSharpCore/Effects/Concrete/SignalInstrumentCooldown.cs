#nullable enable

using MudSharp.GameItems.Components;

namespace MudSharp.Effects.Concrete;

public class SignalInstrumentCooldown : Effect
{
	public SignalInstrumentCooldown(ICharacter owner, SignalInstrumentGameItemComponent instrument) : base(owner)
	{
		Instrument = instrument;
	}

	public SignalInstrumentGameItemComponent Instrument { get; }

	protected override string SpecificEffectType => "SignalInstrumentCooldown";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Unable to signal with {Instrument.Parent.HowSeen(voyeur)} again yet";
	}
}
