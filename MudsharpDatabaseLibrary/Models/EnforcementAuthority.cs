using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EnforcementAuthority
    {
        public EnforcementAuthority()
        {
            EnforcementAuthoritiesAccusableClasses = new HashSet<EnforcementAuthoritiesAccusableClasses>();
            EnforcementAuthoritiesArrestableLegalClasses = new HashSet<EnforcementAuthoritiesArrestableLegalClasses>();
            EnforcementAuthoritiesParentAuthoritiesChild = new HashSet<EnforcementAuthorityParentAuthority>();
            EnforcementAuthoritiesParentAuthoritiesParent = new HashSet<EnforcementAuthorityParentAuthority>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalAuthorityId { get; set; }
        public int Priority { get; set; }
        public bool CanAccuse { get; set; }
        public bool CanForgive { get; set; }
        public bool CanConvict { get; set; }
        public long? FilterProgId { get; set; }

        public virtual FutureProg FilterProg { get; set; }
        public virtual LegalAuthority LegalAuthority { get; set; }
        public virtual ICollection<EnforcementAuthoritiesArrestableLegalClasses> EnforcementAuthoritiesArrestableLegalClasses { get; set; }
        public virtual ICollection<EnforcementAuthoritiesAccusableClasses> EnforcementAuthoritiesAccusableClasses { get; set; }
        public virtual ICollection<EnforcementAuthorityParentAuthority> EnforcementAuthoritiesParentAuthoritiesChild { get; set; }
        public virtual ICollection<EnforcementAuthorityParentAuthority> EnforcementAuthoritiesParentAuthoritiesParent { get; set; }
    }
}
