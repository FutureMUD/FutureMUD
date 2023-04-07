using System.Collections.Generic;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat
{
    public interface ICombatMessageManager
    {
        IEnumerable<ICombatMessage> CombatMessages { get; }
        void AddCombatMessage(ICombatMessage message);
        void RemoveCombatMessage(ICombatMessage message);

        string GetMessageFor(ICharacter character, IPerceiver target, IGameItem weapon, IWeaponAttack attack,
            BuiltInCombatMoveType type, Outcome outcome, IBodypart bodypart);

        string GetFailMessageFor(ICharacter character, IPerceiver target, IGameItem weapon, IWeaponAttack attack,
            BuiltInCombatMoveType type, Outcome outcome, IBodypart bodypart);
    }
}