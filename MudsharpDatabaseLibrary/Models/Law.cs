using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Law
    {
        public Law()
        {
            Crimes = new HashSet<Crime>();
            LawsOffenderClasses = new HashSet<LawsOffenderClasses>();
            LawsVictimClasses = new HashSet<LawsVictimClasses>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalAuthorityId { get; set; }
        public int CrimeType { get; set; }
        public double ActivePeriod { get; set; }
        public string EnforcementStrategy { get; set; }
        public string PunishmentStrategy { get; set; }
        public long? LawAppliesProgId { get; set; }
        public int EnforcementPriority { get; set; }
        public bool CanBeAppliedAutomatically { get; set; }
        public bool DoNotAutomaticallyApplyRepeats { get; set; }
        public bool CanBeArrested { get; set; }
        public bool CanBeOfferedBail { get; set; }

        public virtual FutureProg LawAppliesProg { get; set; }
        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual ICollection<Crime> Crimes { get; set; }
        public virtual ICollection<LawsOffenderClasses> LawsOffenderClasses { get; set; }
        public virtual ICollection<LawsVictimClasses> LawsVictimClasses { get; set; }
    }
}
