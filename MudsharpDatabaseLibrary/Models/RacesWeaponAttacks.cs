using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesWeaponAttacks
    {
        public long RaceId { get; set; }
        public long WeaponAttackId { get; set; }
        public long BodypartId { get; set; }
        public int Quality { get; set; }

        public virtual BodypartProto Bodypart { get; set; }
        public virtual Race Race { get; set; }
        public virtual WeaponAttack WeaponAttack { get; set; }
    }
}
