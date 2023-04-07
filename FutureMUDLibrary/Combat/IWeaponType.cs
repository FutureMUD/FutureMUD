using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems;

namespace MudSharp.Combat {

    public interface IWeaponType : IEditableItem, ISaveable {
        IEnumerable<IWeaponAttack> Attacks { get; }
        ITraitDefinition AttackTrait { get; }
        ITraitDefinition ParryTrait { get; }
        double StaminaPerParry { get; }
        int Reach { get; }
        double ParryBonus { get; }
        WeaponClassification Classification { get; }

        IEnumerable<IWeaponAttack> UsableAttacks(IPerceiver attacker, IGameItem weapon, IPerceiver target,
	        AttackHandednessOptions handedness,
	        bool ignorePosition,
	        params BuiltInCombatMoveType[] type);

        IEnumerable<AttackHandednessOptions> UseableHandednessOptions(ICharacter attacker, IGameItem weapon,
                                                                      IPerceiver target,
                                                                      params BuiltInCombatMoveType[] type);
        
        void AddAttack(IWeaponAttack attack);
        void RemoveAttack(IWeaponAttack attack);
        IWeaponType Clone(string newName);
    }
}