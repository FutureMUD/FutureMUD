using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat
{
    public interface ICombatMessage : ISaveable, IFrameworkItem
    {
        double Chance { get; set; }
        BuiltInCombatMoveType Type { get; set; }
        Outcome? Outcome { get; set; }
        string Message { get; set; }
        string FailMessage { get; set; }
        IFutureProg Prog { get; set; }
        int Priority { get; set; }
        MeleeWeaponVerb? Verb { get; set; }
        HashSet<long> WeaponAttackIds { get; }

        bool Applies(ICharacter character, IPerceiver target, IGameItem weapon,
            IWeaponAttack attack, BuiltInCombatMoveType type, Outcome outcome,
            IBodypart bodypart);

        bool CouldApply(IWeaponAttack attack);
        bool CouldApply(IAuxiliaryCombatAction action);
        string ShowBuilder(ICharacter actor);
        bool BuildingCommand(ICharacter actor, StringStack command);
        void Delete();
    }
}