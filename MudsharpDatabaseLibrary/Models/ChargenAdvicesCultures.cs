using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenAdvicesCultures
    {
        public long ChargenAdviceId { get; set; }
        public long CultureId { get; set; }

        public virtual ChargenAdvice ChargenAdvice { get; set; }
        public virtual Culture Culture { get; set; }
    }
}
