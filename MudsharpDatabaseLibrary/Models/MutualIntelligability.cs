using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MutualIntelligability
    {
        public long ListenerLanguageId { get; set; }
        public long TargetLanguageId { get; set; }
        public int IntelligabilityDifficulty { get; set; }

        public virtual Language ListenerLanguage { get; set; }
        public virtual Language TargetLanguage { get; set; }
    }
}
