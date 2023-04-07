using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LegalClass
    {
        public LegalClass()
        {
            EnforcementAuthoritiesAccusableClasses = new HashSet<EnforcementAuthoritiesAccusableClasses>();
            EnforcementAuthoritiesArrestableClasses = new HashSet<EnforcementAuthoritiesArrestableLegalClasses>();
            LawsOffenderClasses = new HashSet<LawsOffenderClasses>();
            LawsVictimClasses = new HashSet<LawsVictimClasses>();
            WitnessProfilesIgnoredCriminalClasses = new HashSet<WitnessProfilesIgnoredCriminalClasses>();
            WitnessProfilesIgnoredVictimClasses = new HashSet<WitnessProfilesIgnoredVictimClasses>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalAuthorityId { get; set; }
        public int LegalClassPriority { get; set; }
        public long MembershipProgId { get; set; }
        public bool CanBeDetainedUntilFinesPaid { get; set; }

        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual FutureProg MembershipProg { get; set; }
        public virtual ICollection<EnforcementAuthoritiesAccusableClasses> EnforcementAuthoritiesAccusableClasses { get; set; }
        public virtual ICollection<EnforcementAuthoritiesArrestableLegalClasses> EnforcementAuthoritiesArrestableClasses { get; set; }
        public virtual ICollection<LawsOffenderClasses> LawsOffenderClasses { get; set; }
        public virtual ICollection<LawsVictimClasses> LawsVictimClasses { get; set; }
        public virtual ICollection<WitnessProfilesIgnoredCriminalClasses> WitnessProfilesIgnoredCriminalClasses { get; set; }
        public virtual ICollection<WitnessProfilesIgnoredVictimClasses> WitnessProfilesIgnoredVictimClasses { get; set; }
    }
}
