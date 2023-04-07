using System.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Movement;

namespace MudSharp.Effects.Concrete;

public class PassiveInterdiction : Effect, IRangedObstructionEffect, IRemoveOnMovementEffect
{
	public class PassivelyInterceding : Effect, IRemoveOnMeleeCombat, IRemoveOnStateChange, IRemoveOnMovementEffect,
		ILDescSuffixEffect, IScoreAddendumEffect
	{
		public static bool CanInterpose(ICharacter interposer, IPerceivable target)
		{
			var effects = interposer.EffectsOfType<PassivelyInterceding>().ToList();
			if (!effects.Any())
				// Can always interpose at least one target, even if it's bigger than you
			{
				return true;
			}

			var score = effects.Sum(x => 1.0M / (interposer.CurrentContextualSize(SizeContext.RangedTarget) -
				((x.Target as IHaveContextualSizeCategory)?.CurrentContextualSize(SizeContext.RangedTarget) ??
				 interposer.CurrentContextualSize(SizeContext.RangedTarget)) + 1));
			return score + 1.0M / (interposer.CurrentContextualSize(SizeContext.RangedTarget) -
				((target as IHaveContextualSizeCategory)?.CurrentContextualSize(SizeContext.RangedTarget) ??
				 interposer.CurrentContextualSize(SizeContext.RangedTarget)) + 1) <= 1.0M;
		}

		public ICharacter CharacterOwner { get; }
		public PassiveInterdiction AssociatedEffect { get; }

		public PassivelyInterceding(ICharacter owner, IPerceivable target) : base(owner, null)
		{
			Target = target;
			CharacterOwner = owner;
			AssociatedEffect = new PassiveInterdiction(Target, owner);
			Target.AddEffect(AssociatedEffect);
		}

		public IPerceivable Target { get; set; }

		public override string Describe(IPerceiver voyeur)
		{
			return $"Passively interceding in attacks made on {Target.HowSeen(voyeur)}.";
		}

		protected override string SpecificEffectType => "PassivelyInterceding";

		public bool ShouldRemove(CharacterState newState)
		{
			return newState.HasFlag(CharacterState.Dead) || !CharacterState.Able.HasFlag(newState);
		}

		public override void RemovalEffect()
		{
			base.RemovalEffect();
			Target.RemoveEffect(AssociatedEffect);
		}

		bool IRemoveOnMovementEffect.ShouldRemove()
		{
			if (Owner.Location == Target.Location)
			{
				return false;
			}

			if (CharacterOwner.Movement != null && CharacterOwner.Movement == (Target as IMove)?.Movement)
			{
				return false;
			}

			return true;
		}

		#region Implementation of ILDescSuffixEffect

		public string SuffixFor(IPerceiver voyeur)
		{
			return
				$"interposing {Owner.ApparentGender(voyeur).Reflexive()} between {Target.HowSeen(voyeur)} and any ranged attacks";
		}

		public bool SuffixApplies()
		{
			return true;
		}

		#endregion

		#region Implementation of IScoreAddendumEffect

		public bool ShowInScore => true;
		public bool ShowInHealth => false;

		public string ScoreAddendum =>
			$"You are interposing yourself between ranged attacks against {Target.HowSeen(CharacterOwner)}.";

		#endregion
	}

	public ICharacter Intercessor { get; set; }

	protected PassiveInterdiction(IPerceivable owner, ICharacter intercessor) : base(owner, null)
	{
		Intercessor = intercessor;
	}

	protected override string SpecificEffectType => "PassiveInterdiction";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Being passively protected by the intercession of {Intercessor.HowSeen(voyeur)}.";
	}

	public override bool Applies(object target)
	{
		return IsObstructedFrom(Owner, Intercessor, target as IPerceivable);
	}

	public IPerceivable Obstruction => Intercessor;

	public bool IsObstructedFrom(IPerceivable target, IPerceivable intercessor, [CanBeNull] IPerceivable attackOrigin)
	{
		if (intercessor != Intercessor || target != Owner || attackOrigin == Intercessor ||
		    attackOrigin == target)
		{
			return false;
		}

		if (target is ICombatant c1 && attackOrigin is ICombatant c2 &&
		    (c1.CombatTarget == c2 || c2.CombatTarget == c1) && c1.IsEngagedInMelee &&
		    c2.IsEngagedInMelee)
		{
			return false;
		}

		var targetSize = (target as IHaveContextualSizeCategory)?.CurrentContextualSize(SizeContext.RangedTarget) ??
		                 GameItems.SizeCategory.Normal;
		var intercessorSize =
			(intercessor as IHaveContextualSizeCategory)?.CurrentContextualSize(SizeContext.RangedTarget) ??
			GameItems.SizeCategory.Normal;
		var diff = (int)targetSize - (int)intercessorSize;
		var chance = 0.0;
		if (diff <= -2)
		{
			chance = 1.0;
		}
		else
		{
			switch (diff)
			{
				case -1:
					chance = 0.9;
					break;
				case 0:
					chance = 0.75;
					break;
				case 1:
					chance = 0.5;
					break;
				case 2:
					chance = 0.33;
					break;
				case 3:
					chance = 0.1;
					break;
			}
		}

		return RandomUtilities.Roll(1.0, chance);
	}

	bool IRemoveOnMovementEffect.ShouldRemove()
	{
		if (Owner.Location == Intercessor.Location)
		{
			return false;
		}

		if (Intercessor.Movement != null && Intercessor.Movement == (Owner as IMove)?.Movement)
		{
			return false;
		}

		return true;
	}

	public override void RemovalEffect()
	{
		base.RemovalEffect();
		Intercessor.RemoveAllEffects(x => x.GetSubtype<PassivelyInterceding>()?.Target == Owner);
	}
}