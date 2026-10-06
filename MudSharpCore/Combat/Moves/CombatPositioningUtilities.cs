using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.NPC.AI;

namespace MudSharp.Combat.Moves;

public static class CombatPositioningUtilities
{
	public static void WorsenCombatPosition(ICharacter assailant, ICharacter defender)
	{
		if (!CommandExecutionScope.TryContinue()) return;
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().ToArray().FirstOrDefault(x => x.AppliesTo(defender));
		if (!CommandExecutionScope.TryContinue()) return;
		if (effect is not null)
		{
			switch (effect.Facing)
			{
				case Facing.Front:
					return;
				case Facing.Rear:
					CommandExecutionScope.MarkCommitted();
					assailant.RemoveEffect(effect);
					if (!CommandExecutionScope.TryContinue()) return;
					assailant.AddEffect(new FixedCombatFacing(assailant, defender,
						RandomUtilities.Random(1, 2) == 1 ? Facing.LeftFlank : Facing.RightFlank));
					return;
				case Facing.LeftFlank:
				case Facing.RightFlank:
					CommandExecutionScope.MarkCommitted();
					assailant.RemoveEffect(effect);
					return;
			}
		}
	}

	public static void ImproveCombatPosition(ICharacter assailant, ICharacter defender)
	{
		if (!CommandExecutionScope.TryContinue()) return;
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().ToArray().FirstOrDefault(x => x.AppliesTo(defender));
		if (!CommandExecutionScope.TryContinue()) return;
		if (effect is not null)
		{
			switch (effect.Facing)
			{
				case Facing.Rear:
					return;
				case Facing.LeftFlank:
				case Facing.RightFlank:
					CommandExecutionScope.MarkCommitted();
					assailant.AddEffect(new FixedCombatFacing(assailant, defender, Facing.Rear));
					if (!CommandExecutionScope.TryContinue()) return;
					assailant.RemoveEffect(effect);
					return;
			}
		}

		CommandExecutionScope.MarkCommitted();
		assailant.AddEffect(new FixedCombatFacing(assailant, defender,
			RandomUtilities.Random(1, 2) == 1 ? Facing.LeftFlank : Facing.RightFlank));
	}
}
