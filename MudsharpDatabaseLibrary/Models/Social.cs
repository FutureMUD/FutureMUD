using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Social
    {
        public string Name { get; set; }
        public string NoTargetEcho { get; set; }
        public string OneTargetEcho { get; set; }
        public long? FutureProgId { get; set; }
        public string DirectionTargetEcho { get; set; }
        public string MultiTargetEcho { get; set; }

        public virtual FutureProg FutureProg { get; set; }
    }
}
