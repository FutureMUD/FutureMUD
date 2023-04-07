using MudSharp.Character;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class EnvenomingAttackMove : NaturalAttackMove
{
	public EnvenomingAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack,
		target)
	{
		Target = target;
	}

	public ICharacter Target { get; }

	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.EnvenomingAttack;

	public override string Description => $"An envenoming natural attack";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var result = base.ResolveMove(defenderMove);
		var envenoming = (IEnvenomingAttack)Attack;
		if (result.WoundsCaused.Any(x => x.Parent == Target && x.Severity >= envenoming.MinimumWoundSeverity))
		{
			var multiplier = result.AttackerOutcome switch
			{
				Outcome.MajorPass => 1.0,
				Outcome.Pass => 0.8,
				Outcome.MinorPass => 0.5,
				Outcome.MinorFail => 0.3,
				Outcome.Fail => 0.1,
				Outcome.MajorFail => 0.0,
				_ => 1.0
			};
			Target.Body.HealthStrategy.InjectedLiquid(Target,
				new Form.Material.LiquidMixture(envenoming.Liquid, envenoming.MaximumQuantity * multiplier, Gameworld));
		}

		return result;
	}
}