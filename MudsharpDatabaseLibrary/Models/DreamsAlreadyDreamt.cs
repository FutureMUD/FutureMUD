using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class DreamsAlreadyDreamt
    {
        public long DreamId { get; set; }
        public long CharacterId { get; set; }

        public virtual Character Character { get; set; }
        public virtual Dream Dream { get; set; }
    }
}
