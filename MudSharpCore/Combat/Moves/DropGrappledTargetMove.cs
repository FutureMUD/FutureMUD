using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class DropGrappledTargetMove : CombatMoveBase
{
	public DropGrappledTargetMove(ICharacter assailant, ICharacter target)
	{
		Assailant = assailant;
		CharacterTarget = target;
		_characterTargets.Add(target);
	}

	public ICharacter CharacterTarget { get; }

	public override string Description => "Dropping a grappled target";
	public override Difficulty RecoveryDifficultySuccess => Difficulty.Easy;
	public override Difficulty RecoveryDifficultyFailure => Difficulty.Easy;
	public override ExertionLevel AssociatedExertion => ExertionLevel.Low;
	public override double BaseDelay => 0.5;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var grapples = Assailant.EffectsOfType<IGrappling>()
		                       .Where(x => x.Target == CharacterTarget)
		                       .Select(x => x as Grappling)
		                       .Where(x => x is not null)
		                       .ToList();
		if (!grapples.Any())
		{
			return new CombatMoveResult { RecoveryDifficulty = RecoveryDifficultyFailure };
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ release|releases $1 from &0's grasp.",
				Assailant, Assailant, CharacterTarget),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		foreach (var grapple in grapples)
		{
			Assailant.RemoveEffect(grapple, true);
		}

		CombatForcedMovementUtilities.ApplyFallIfNeeded(CharacterTarget, false);
		return new CombatMoveResult
		{
			MoveWasSuccessful = true,
			RecoveryDifficulty = RecoveryDifficultySuccess,
			AttackerOutcome = Outcome.NotTested,
			DefenderOutcome = Outcome.NotTested
		};
	}
}
