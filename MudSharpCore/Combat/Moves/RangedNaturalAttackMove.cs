using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class RangedNaturalAttackMove : NaturalRangedAttackMoveBase
{
    public RangedNaturalAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
    {
    }

    public override string Description => "Making a ranged natural attack";
    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.RangedNaturalAttack;
    protected override CheckType RangedCheck => CheckType.RangedNaturalAttack;
}
