using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemProtosTags
    {
        public long GameItemProtoId { get; set; }
        public long TagId { get; set; }
        public int GameItemProtoRevisionNumber { get; set; }

        public virtual GameItemProto GameItemProto { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
