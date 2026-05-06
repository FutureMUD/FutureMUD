using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;

namespace MudSharp.Combat.Moves;

public static class CombatPositioningUtilities
{
	public static void WorsenCombatPosition(ICharacter assailant, ICharacter defender)
	{
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().FirstOrDefault(x => x.AppliesTo(defender));
		if (effect is not null)
		{
			switch (effect.Facing)
			{
				case Facing.Front:
					return;
				case Facing.Rear:
					assailant.RemoveEffect(effect);
					assailant.AddEffect(new FixedCombatFacing(assailant, defender,
						RandomUtilities.Random(1, 2) == 1 ? Facing.LeftFlank : Facing.RightFlank));
					return;
				case Facing.LeftFlank:
				case Facing.RightFlank:
					assailant.RemoveEffect(effect);
					return;
			}
		}
	}

	public static void ImproveCombatPosition(ICharacter assailant, ICharacter defender)
	{
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().FirstOrDefault(x => x.AppliesTo(defender));
		if (effect is not null)
		{
			switch (effect.Facing)
			{
				case Facing.Rear:
					return;
				case Facing.LeftFlank:
				case Facing.RightFlank:
					assailant.AddEffect(new FixedCombatFacing(assailant, defender, Facing.Rear));
					assailant.RemoveEffect(effect);
					return;
			}
		}

		assailant.AddEffect(new FixedCombatFacing(assailant, defender,
			RandomUtilities.Random(1, 2) == 1 ? Facing.LeftFlank : Facing.RightFlank));
	}
}
