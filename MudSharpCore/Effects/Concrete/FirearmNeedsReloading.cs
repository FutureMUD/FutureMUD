using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Effects.Concrete;

public class FirearmNeedsReloading : Effect, IEffectSubtype
{
	public FirearmNeedsReloading(ICharacter owner, IRangedWeapon weapon, IFutureProg applicabilityProg = null) : base(
		owner, applicabilityProg)
	{
		Firearm = weapon;
	}

	public IRangedWeapon Firearm { get; set; }

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return "A firearm needs reloading.";
	}

	protected override string SpecificEffectType { get; } = "FirearmNeedsReloading";

	#endregion
}