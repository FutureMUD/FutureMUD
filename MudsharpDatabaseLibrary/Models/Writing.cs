using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Writing
    {
        public long Id { get; set; }
        public string WritingType { get; set; }
        public int Style { get; set; }
        public long LanguageId { get; set; }
        public long ScriptId { get; set; }
        public long AuthorId { get; set; }
        public long? TrueAuthorId { get; set; }
        public double HandwritingSkill { get; set; }
        public double LiteracySkill { get; set; }
        public double ForgerySkill { get; set; }
        public double LanguageSkill { get; set; }
        public string Definition { get; set; }
        public long WritingColour { get; set; }
        public int ImplementType { get; set; }

        public virtual Character Author { get; set; }
        public virtual Language Language { get; set; }
        public virtual Script Script { get; set; }
        public virtual Character TrueAuthor { get; set; }
    }
}
