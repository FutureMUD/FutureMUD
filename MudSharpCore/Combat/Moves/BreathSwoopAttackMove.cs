using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
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

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        ICell startingCell = Assailant.Location;
        RoomLayer startingLayer = Assailant.RoomLayer;

        if (Assailant.Location != Target.Location)
        {
            List<ICellExit> path = Assailant.PathBetween(Target, 10, false, false, true).ToList();
            if (path.Any())
            {
                ICellExit exit = path.First();
                Assailant.OutputHandler.Handle(new EmoteOutput(
                    new Emote("@ swoop|swoops in on $0 with a deep breath.", Assailant, Assailant, Target),
                    flags: OutputFlags.SuppressObscured));
                Assailant.MoveTo(exit.Destination, Target.RoomLayer, exit);
            }
        }
        else if (Assailant.RoomLayer != Target.RoomLayer)
        {
            Assailant.RoomLayer = Target.RoomLayer;
        }

        Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("AdvantagePerLayerSwoopAttack");
        CombatMoveResult result = base.ResolveMove(defenderMove);
        if (startingCell != Assailant.Location || startingLayer != Assailant.RoomLayer)
        {
            Assailant.MoveTo(startingCell, startingLayer);
        }

        return result;
    }
}
