using MudSharp.Combat.Moves;
using MudSharp.Magic.Powers;

namespace MudSharp.Combat.Strategies;

public class MeleeMagicStrategy : StandardMeleeStrategy
{
    public new static MeleeMagicStrategy Instance => new();

    protected MeleeMagicStrategy()
    {
    }

    public override CombatStrategyMode Mode => CombatStrategyMode.MeleeMagic;

    protected override ICombatMove AttemptUseMagic(ICharacter combatant) => base.AttemptUseMagic(combatant) ?? HandleWeaponAttackRolled(combatant);
    protected override ICombatMove AttemptUsePsychicAbility(ICharacter combatant) => base.AttemptUsePsychicAbility(combatant) ?? HandleWeaponAttackRolled(combatant);
}
