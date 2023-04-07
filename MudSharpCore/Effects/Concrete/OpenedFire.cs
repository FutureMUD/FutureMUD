using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class OpenedFire : CombatEffectBase
{
	public OpenedFire(ICharacter owner) : base(owner, owner.Combat, null)
	{
	}

	protected override string SpecificEffectType => "OpenedFire";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Has opened fire on a target.";
	}
}