using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenAdvicesRaces
    {
        public long ChargenAdviceId { get; set; }
        public long RaceId { get; set; }

        public virtual ChargenAdvice ChargenAdvice { get; set; }
        public virtual Race Race { get; set; }
    }
}
