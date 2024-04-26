using System.Collections.Generic;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Effects.Concrete;

public class SneakMove : Effect, ISneakMoveEffect
{
	private readonly List<IEffect> _sawSneakers = new();

	public SneakMove(IPerceivable owner)
		: base(owner)
	{
	}

	protected override string SpecificEffectType => "SneakMove";

	public override string Describe(IPerceiver voyeur)
	{
		return "Currently making a sneaky movement";
	}

	public override bool Applies(object target)
	{
		if (target is not IPerceiver voyeur)
		{
			return false;
		}

		return voyeur.AffectedBy<ISawSneakerEffect>(Owner);
	}

	#region ISneakMoveEffect Members

	public void RegisterSawSneaker(IEffect effect)
	{
		_sawSneakers.Add(effect);
	}

	public bool Subtle { get; init; }

	#endregion

	public override void RemovalEffect()
	{
		foreach (var effect in _sawSneakers)
		{
			effect.Owner.RemoveEffect(effect);
		}
	}
}