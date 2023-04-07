using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class Rescue : CombatEffectBase, IRescueEffect, IRemoveOnCombatEnd, IAffectProximity
{
	public Rescue(ICharacter owner, ICharacter target) : base(owner, owner.Combat)
	{
		RescueTarget = target;
	}

	protected override string SpecificEffectType => "Rescue";

	public ICharacter RescueTarget { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Attempting to rescue {RescueTarget.HowSeen(voyeur)}.";
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (RescueTarget == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}
}