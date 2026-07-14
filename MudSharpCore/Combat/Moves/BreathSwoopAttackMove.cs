using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Combat.Moves;

public class BreathSwoopAttackMove : BreathWeaponAttackMove
{
    public BreathSwoopAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
    {
        Target = target;
    }

	public ICharacter Target { get; }
	public override string Description => "Performing a breath-weapon swoop";
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.SwoopAttackUnarmed;
	protected override CheckType RangedCheck => CheckType.BreathWeaponSwoop;

	protected override string BuildAttackEmote(IPerceiver target, Outcome outcome)
	{
		return string.Format(
			Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, Attack,
				BuiltInCombatMoveType.BreathWeaponAttack, outcome, Bodypart),
			Bodypart.FullDescription(), TargetBodypart?.FullDescription() ?? "body");
	}

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        ICell startingCell = Assailant.Location;
        RoomLayer startingLayer = Assailant.RoomLayer;

		if (Assailant.Location != Target.Location)
		{
			List<ICellExit> path = Assailant.PathBetween(Target, 1, false, false, true)?.ToList() ?? [];
			if (path.Count != 1)
			{
				return CombatMoveResult.Irrelevant;
			}

			ICellExit exit = path[0];
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote("@ swoop|swoops in on $0 with a deep breath.", Assailant, Assailant, Target),
				flags: OutputFlags.SuppressObscured));
			Assailant.MoveTo(exit.Destination, Target.RoomLayer, exit);
			if (Assailant.Location != Target.Location)
			{
				Assailant.MoveTo(startingCell, startingLayer);
				return CombatMoveResult.Irrelevant;
			}
		}
		else if (Assailant.RoomLayer != Target.RoomLayer)
		{
			if (!Assailant.CouldTransitionToLayer(Target.RoomLayer))
			{
				return CombatMoveResult.Irrelevant;
			}

			Assailant.RoomLayer = Target.RoomLayer;
		}

		Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("AdvantagePerLayerSwoopAttack");
		try
		{
			return base.ResolveMove(defenderMove);
		}
		finally
		{
			if (startingCell != Assailant.Location || startingLayer != Assailant.RoomLayer)
			{
				Assailant.MoveTo(startingCell, startingLayer);
			}
		}
	}
}
