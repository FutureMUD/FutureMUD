using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class AIFleeing : Effect, IEffectSubtype
{
	public List<ICell> PotentialFleeLocations { get; }

	public AIFleeing(ICharacter owner, IEnumerable<ICell> fleelocations) : base(owner)
	{
		PotentialFleeLocations = new List<ICell>(fleelocations);
	}

	protected override string SpecificEffectType => "AIFleeing";

	public override string Describe(IPerceiver voyeur)
	{
		return "An undescribed effect of type AIFleeing.";
	}
}