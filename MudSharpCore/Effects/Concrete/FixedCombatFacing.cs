using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System.Xml.Linq;

namespace MudSharp.Effects.Concrete;

public class FixedCombatFacing : CombatEffectBase, IFixedFacingEffect
{
	public FixedCombatFacing(IPerceivable owner, IPerceiver combatant, Facing facing,
		IFutureProg applicabilityProg = null) : base(owner, combatant.Combat, applicabilityProg)
	{
		Combatant = combatant;
		Facing = facing;
	}

	public FixedCombatFacing(XElement effect, IPerceivable owner) : base(effect, owner)
	{
	}

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Fixed facing of {Facing.Describe()} against {Combatant.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType { get; } = "Fixed Combat Facing";

	#endregion

	#region Implementation of IFixedFacingEffect

	public bool AppliesTo(ICombatant combatant)
	{
		return combatant == Combatant;
	}

	public Facing Facing { get; set; }
	public IPerceiver Combatant { get; set; }

	#endregion
}