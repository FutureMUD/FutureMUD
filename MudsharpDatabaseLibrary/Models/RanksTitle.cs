using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RanksTitle
    {
        public long RankId { get; set; }
        public string Title { get; set; }
        public long? FutureProgId { get; set; }
        public int Order { get; set; }

        public virtual FutureProg FutureProg { get; set; }
        public virtual Rank Rank { get; set; }
    }
}
