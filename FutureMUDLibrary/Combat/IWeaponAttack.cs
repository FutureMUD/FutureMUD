using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat {
    public interface IWeaponAttack : IKeywordedItem {
        MeleeWeaponVerb Verb { get; set; }
        BuiltInCombatMoveType MoveType { get; set; }
        CombatMoveIntentions Intentions { get; set; }
        Difficulty RecoveryDifficultyFailure { get; set; }
        Difficulty RecoveryDifficultySuccess { get; set; }
        ExertionLevel ExertionLevel { get; set; }
        double StaminaCost { get; set; }
        double BaseDelay { get; set; }
        double Weighting { get; set; }
        IBodypartShape BodypartShape { get; set; }
        IDamageProfile Profile { get; set; }
        IFutureProg UsabilityProg { get; set; }
        Orientation Orientation { get; set; }
        Alignment Alignment { get; set; }
        AttackHandednessOptions HandednessOptions { get; set; }
        T GetAttackType<T>() where T : class, IWeaponAttack;
        IEnumerable<IPositionState> RequiredPositionStates { get; }
        bool IsAttackType<T>() where T : class, IWeaponAttack;
        bool UsableAttack(IPerceiver attacker, IGameItem weapon, IPerceiver target, AttackHandednessOptions handedness,
	        bool ignorePosition, params BuiltInCombatMoveType[] type);

        string DescribeForAttacksCommand(ICharacter actor);

        string ShowBuilder(ICharacter actor);
        bool BuildingCommand(ICharacter actor, StringStack command);
        IWeaponAttack CloneWeaponAttack();
        string SpecialListText { get; }
    }
}