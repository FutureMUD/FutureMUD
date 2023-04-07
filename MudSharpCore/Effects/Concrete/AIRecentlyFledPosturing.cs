using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class AIRecentlyFledPosturing : Effect, IEffectSubtype
{
	public double PreviousThreat { get; set; }

	public AIRecentlyFledPosturing(ICharacter owner, double previousThreat) : base(owner)
	{
		PreviousThreat = previousThreat;
	}

	protected override string SpecificEffectType => "AIRecentlyFledPosturing";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Recently fled from posturing with a threat level of {PreviousThreat}.";
	}
}