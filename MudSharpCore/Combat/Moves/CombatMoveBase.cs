using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public abstract class CombatMoveBase : ICombatMove
{
	private ICharacter _assailant;

	protected HashSet<ICharacter> _characterTargets = new();

	protected HashSet<IPerceiver> _targets = new();

	public ICharacter Assailant
	{
		get => _assailant;
		init
		{
			_assailant = value;
			if (value.CombatTarget is ICharacter item && !_characterTargets.Contains(item))
			{
				_characterTargets.Add(item);
			}
		}
	}

	public virtual ExertionLevel AssociatedExertion { get; } = ExertionLevel.Rest;

	public virtual CheckType Check { get; } = CheckType.CombatMoveCheck;

	public virtual Difficulty CheckDifficulty { get; } = Difficulty.Normal;

	public abstract string Description { get; }

	public virtual Difficulty RecoveryDifficultyFailure { get; } = Difficulty.Hard;

	public virtual Difficulty RecoveryDifficultySuccess { get; } = Difficulty.Normal;

	public virtual double StaminaCost { get; } = 0;

	public abstract CombatMoveResult ResolveMove(ICombatMove defenderMove);

	public IEnumerable<ICharacter> CharacterTargets => _characterTargets;

	public IEnumerable<IPerceiver> Targets => _targets;

	public virtual IPerceiver PrimaryTarget
	{
		get => _targets.FirstOrDefault() ?? 
		       Assailant.CombatTarget ?? 
		       Assailant.Combat.Combatants.FirstOrDefault(x => x.CombatTarget == Assailant)
		       ;
		set
		{
			_targets.Add(value);
			if (value is ICharacter ch)
			{
				_characterTargets.Add(ch);
			}
		}
	}

	public IFuturemud Gameworld => Assailant.Gameworld;

	public virtual double BaseDelay { get; } = 0.2;

	public override string ToString()
	{
		return Description;
	}

	protected int GetPositionPenalty(Facing facing)
	{
		switch (facing)
		{
			case Facing.Front:
				return 0;
			case Facing.LeftFlank:
			case Facing.RightFlank:
				return 2;
			case Facing.Rear:
				return 4;
		}

		return 0;
	}

	protected void WorsenCombatPosition(ICharacter assailant, ICharacter defender)
	{
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().FirstOrDefault(x => x.AppliesTo(defender));
		if (effect != null)
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

	protected void ImproveCombatPosition(ICharacter assailant, ICharacter defender)
	{
		var effect = assailant.EffectsOfType<IFixedFacingEffect>().FirstOrDefault(x => x.AppliesTo(defender));
		if (effect != null)
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

	public virtual bool UsesStaminaWithResult(CombatMoveResult result)
	{
		return true;
	}
}