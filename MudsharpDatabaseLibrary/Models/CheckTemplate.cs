using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CheckTemplate
    {
        public CheckTemplate()
        {
            CheckTemplateDifficulties = new HashSet<CheckTemplateDifficulty>();
            Checks = new HashSet<Check>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string CheckMethod { get; set; }
        public bool ImproveTraits { get; set; }
        public short FailIfTraitMissingMode { get; set; }
        public bool CanBranchIfTraitMissing { get; set; }

        public virtual ICollection<CheckTemplateDifficulty> CheckTemplateDifficulties { get; set; }
        public virtual ICollection<Check> Checks { get; set; }
    }
}
