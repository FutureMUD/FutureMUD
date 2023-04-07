using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RanksAbbreviations
    {
        public long RankId { get; set; }
        public string Abbreviation { get; set; }
        public long? FutureProgId { get; set; }
        public int Order { get; set; }

        public virtual FutureProg FutureProg { get; set; }
        public virtual Rank Rank { get; set; }
    }
}
