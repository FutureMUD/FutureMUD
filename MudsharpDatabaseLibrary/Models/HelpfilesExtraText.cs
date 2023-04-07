using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class HelpfilesExtraText
    {
        public long HelpfileId { get; set; }
        public string Text { get; set; }
        public long RuleId { get; set; }
        public int DisplayOrder { get; set; }

        public virtual Helpfile Helpfile { get; set; }
        public virtual FutureProg Rule { get; set; }
    }
}
