using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.Magic.Powers
{
    public interface IMagicAttackPower : IMagicPower
    {
        BuiltInCombatMoveType MoveType { get; }
        CombatMoveIntentions PowerIntentions { get; }
        bool CanInvokePower(ICharacter invoker, ICharacter target);
        void UseAttackPower(IMagicPowerAttackMove move);
        double BaseDelay { get; }
        ExertionLevel ExertionLevel { get; }
        double StaminaCost { get; }
        double Weighting { get; }
        IEnumerable<DefenseType> ValidDefenseTypes { get; }
        Orientation Orientation { get; }
        Alignment Alignment { get; }
        Difficulty BaseBlockDifficulty { get; }
        Difficulty BaseParryDifficulty { get; }
        Difficulty BaseDodgeDifficulty { get; }
        IWeaponAttack WeaponAttack { get; }
        ITraitDefinition AttackerTrait { get; }
        int Reach { get; }
    }
}
