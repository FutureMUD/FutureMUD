using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemProtosOnLoadProgs
    {
        public long GameItemProtoId { get; set; }
        public int GameItemProtoRevisionNumber { get; set; }
        public long FutureProgId { get; set; }

        public virtual FutureProg FutureProg { get; set; }
        public virtual GameItemProto GameItemProto { get; set; }
    }
}
