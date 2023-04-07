using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class RecentlyRescuedTarget : CombatEffectBase, IRecentlyRescuedTargetEffect
{
	public RecentlyRescuedTarget(ICharacter owner, ICharacter target, ICharacter rescuer)
		: base(owner, owner.Combat)
	{
		Rescued = target;
		Rescuer = rescuer;
	}

	protected override string SpecificEffectType => "Recently Rescued Target";

	public ICharacter Rescued { get; set; }

	public ICharacter Rescuer { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Recently had {Rescued.HowSeen(voyeur)} rescued from them by {Rescuer.HowSeen(voyeur)}.";
	}
}