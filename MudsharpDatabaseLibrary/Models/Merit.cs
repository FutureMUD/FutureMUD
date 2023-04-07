using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Merit
    {
        public Merit()
        {
            ChargenRolesMerits = new HashSet<ChargenRolesMerit>();
            InverseParent = new HashSet<Merit>();
            MeritsChargenResources = new HashSet<MeritsChargenResources>();
            PerceiverMerits = new HashSet<PerceiverMerit>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int MeritType { get; set; }
        public int MeritScope { get; set; }
        public string Definition { get; set; }
        public long? ParentId { get; set; }

        public virtual Merit Parent { get; set; }
        public virtual ICollection<ChargenRolesMerit> ChargenRolesMerits { get; set; }
        public virtual ICollection<Merit> InverseParent { get; set; }
        public virtual ICollection<MeritsChargenResources> MeritsChargenResources { get; set; }
        public virtual ICollection<PerceiverMerit> PerceiverMerits { get; set; }
    }
}
