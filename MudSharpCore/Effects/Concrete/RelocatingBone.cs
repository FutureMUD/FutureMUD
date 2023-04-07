using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Effects.Concrete;

public class RelocatingBone : CharacterActionWithTarget, IAffectProximity
{
	private static string _effectDurationDiceExpression;

	private static string EffectDurationDiceExpression
	{
		get
		{
			if (_effectDurationDiceExpression == null)
			{
				_effectDurationDiceExpression = Futuremud.Games.First()
				                                         .GetStaticConfiguration("BindingEffectDurationDiceExpression");
			}

			return _effectDurationDiceExpression;
		}
	}

	public IBone Bone { get; set; }

	public static TimeSpan EffectDuration => TimeSpan.FromSeconds(Dice.Roll(EffectDurationDiceExpression));

	#region Overrides of Effect

	public override string Describe(IPerceiver voyeur)
	{
		return $"Relocating a bone of {TargetCharacter.HowSeen(voyeur)}.";
	}

	protected override string SpecificEffectType => "RelocatingBone";

	#endregion

	public RelocatingBone(ICharacter owner, ICharacter target, IBone bone) : base(owner, target)
	{
		Bone = bone;
		WhyCannotMoveEmoteString = $"@ cannot move because $0 $0|are|is relocating $1's {Bone.FullDescription()}.";
		CancelEmoteString = $"@ $0|stop|stops relocating $1's {Bone.FullDescription()}.";
		LDescAddendum = $"relocating $1's {Bone.FullDescription()}";
		ActionDescription = $"relocating $1's {Bone.FullDescription()}";
		_blocks.Add("general");
		_blocks.Add("movement");
	}

	public (bool Affects, Proximity Proximity) GetProximityFor(IPerceivable thing)
	{
		if (TargetCharacter == thing)
		{
			return (true, Proximity.Immediate);
		}

		return (false, Proximity.Unapproximable);
	}

	#region Overrides of TargetedBlockingDelayedAction

	/// <summary>
	///     Fires when an effect is removed, including a matured scheduled effect
	/// </summary>
	public override void RemovalEffect()
	{
		ReleaseEventHandlers();
	}

	#endregion

	public override void ExpireEffect()
	{
		var wound = TargetCharacter.Wounds.Where(x => x.Bodypart == Bone).OfType<BoneFracture>()
		                           .OrderBy(x => x.CanBeTreated(TreatmentType.Relocation)).FirstOrDefault();
		if (wound == null)
		{
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ stop|stops relocating $1's {Bone.FullDescription()} because it no longer requires that treatment.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var difficulty = wound.CanBeTreated(TreatmentType.Relocation);
		if (difficulty == RPG.Checks.Difficulty.Impossible)
		{
			CharacterOwner.OutputHandler.Send(wound.WhyCannotBeTreated(TreatmentType.Relocation));
			CharacterOwner.OutputHandler.Handle(new EmoteOutput(new Emote(
				$"@ stop|stops relocating $1's {Bone.FullDescription()} because it no longer requires that treatment.",
				CharacterOwner, CharacterOwner, TargetCharacter)));
			Owner.RemoveEffect(this, true);
			return;
		}

		var check = Gameworld.GetCheck(RPG.Checks.CheckType.RelocateBoneCheck);
		var result = check.Check(CharacterOwner, difficulty, Target);
		wound.Treat(CharacterOwner, TreatmentType.Relocation, null, result.Outcome, false);
		Owner.RemoveEffect(this, true);
	}
}