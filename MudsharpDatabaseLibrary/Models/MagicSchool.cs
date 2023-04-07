using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicSchool
    {
        public MagicSchool()
        {
            InverseParentSchool = new HashSet<MagicSchool>();
            MagicCapabilities = new HashSet<MagicCapability>();
            MagicPowers = new HashSet<MagicPower>();
            MagicSpells = new HashSet<MagicSpell>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentSchoolId { get; set; }
        public string SchoolVerb { get; set; }
        public string SchoolAdjective { get; set; }
        public string PowerListColour { get; set; }

        public virtual MagicSchool ParentSchool { get; set; }
        public virtual ICollection<MagicSchool> InverseParentSchool { get; set; }
        public virtual ICollection<MagicCapability> MagicCapabilities { get; set; }
        public virtual ICollection<MagicPower> MagicPowers { get; set; }
        public virtual ICollection<MagicSpell> MagicSpells { get; set; }
    }
}
