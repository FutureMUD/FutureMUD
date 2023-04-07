using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CombatMessagesWeaponAttacks
    {
        public long CombatMessageId { get; set; }
        public long WeaponAttackId { get; set; }

        public virtual CombatMessage CombatMessage { get; set; }
        public virtual WeaponAttack WeaponAttack { get; set; }
    }
}
