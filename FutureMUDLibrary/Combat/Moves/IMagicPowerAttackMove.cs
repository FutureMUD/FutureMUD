using MudSharp.Magic.Powers;

namespace MudSharp.Combat.Moves
{
    public interface IMagicPowerAttackMove : IWeaponAttackMove
    {
        IMagicAttackPower AttackPower { get; }
    }
}