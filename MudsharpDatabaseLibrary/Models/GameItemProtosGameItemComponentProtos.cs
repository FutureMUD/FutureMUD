using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemProtosGameItemComponentProtos
    {
        public long GameItemProtoId { get; set; }
        public long GameItemComponentProtoId { get; set; }
        public int GameItemProtoRevision { get; set; }
        public int GameItemComponentRevision { get; set; }

        public virtual GameItemComponentProto GameItemComponent { get; set; }
        public virtual GameItemProto GameItemProto { get; set; }
    }
}
