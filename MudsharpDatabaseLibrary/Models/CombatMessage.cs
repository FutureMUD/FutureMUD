﻿using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CombatMessage
    {
        public CombatMessage()
        {
            CombatMessagesWeaponAttacks = new HashSet<CombatMessagesWeaponAttacks>();
			CombatMessagesCombatActions = new HashSet<CombatMessagesCombatActions>();

		}

        public long Id { get; set; }
        public int Type { get; set; }
        public int? Outcome { get; set; }
        public string Message { get; set; }
        public long? ProgId { get; set; }
        public long? AuxiliaryProgId { get; set; }
        public int Priority { get; set; }
        public int? Verb { get; set; }
        public double Chance { get; set; }
        public string FailureMessage { get; set; }

        public virtual FutureProg Prog { get; set; }
        public virtual FutureProg AuxiliaryProg { get; set; }
        public virtual ICollection<CombatMessagesWeaponAttacks> CombatMessagesWeaponAttacks { get; set; }
		public virtual ICollection<CombatMessagesCombatActions> CombatMessagesCombatActions { get; set; }
	}
}
