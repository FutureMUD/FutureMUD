using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemComponent
    {
        public long Id { get; set; }
        public long GameItemComponentProtoId { get; set; }
        public int GameItemComponentProtoRevision { get; set; }
        public string Definition { get; set; }
        public long GameItemId { get; set; }

        public virtual GameItem GameItem { get; set; }
    }
}
