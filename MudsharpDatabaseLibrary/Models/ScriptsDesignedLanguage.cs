using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ScriptsDesignedLanguage
    {
        public long ScriptId { get; set; }
        public long LanguageId { get; set; }

        public virtual Language Language { get; set; }
        public virtual Script Script { get; set; }
    }
}
