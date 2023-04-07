using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Helpfile
    {
        public Helpfile()
        {
            HelpfilesExtraTexts = new HashSet<HelpfilesExtraText>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string TagLine { get; set; }
        public string PublicText { get; set; }
        public long? RuleId { get; set; }
        public string Keywords { get; set; }
        public string LastEditedBy { get; set; }
        public DateTime LastEditedDate { get; set; }

        public virtual FutureProg Rule { get; set; }
        public virtual ICollection<HelpfilesExtraText> HelpfilesExtraTexts { get; set; }
    }
}
