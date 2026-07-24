#nullable enable

namespace MudSharp.Effects.Concrete;

public class RecognisedMilitaryStandard : Effect
{
	public RecognisedMilitaryStandard(ICharacter owner, long standardId) : base(owner)
	{
		StandardId = standardId;
	}

	public long StandardId { get; }

	protected override string SpecificEffectType => "RecognisedMilitaryStandard";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Recognises military standard #{StandardId.ToString("N0", voyeur)}";
	}
}
