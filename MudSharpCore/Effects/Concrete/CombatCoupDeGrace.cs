using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class CombatCoupDeGrace : CombatEffectBase, ICombatEffectRemovedOnTargetChange
{
	public CombatCoupDeGrace(IPerceivable owner, ICombat combat, IMeleeWeapon weapon,
		IFixedBodypartWeaponAttack attack, PlayerEmote emote, IFutureProg applicabilityProg = null)
		: base(owner, combat, applicabilityProg)
	{
		Weapon = weapon;
		Attack = attack;
		Emote = emote;
	}

	public IMeleeWeapon Weapon { get; set; }
	public IFixedBodypartWeaponAttack Attack { get; set; }
	public PlayerEmote Emote { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Wants to Coup-De-Grace";
	}

	protected override string SpecificEffectType => "CombatCoupDeGrace";

	#endregion
}