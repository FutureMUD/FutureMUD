using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class PerceiverMerit
    {
        public long Id { get; set; }
        public long MeritId { get; set; }
        public long? BodyId { get; set; }
        public long? CharacterId { get; set; }
        public long? GameItemId { get; set; }

        public virtual Body Body { get; set; }
        public virtual Character Character { get; set; }
        public virtual Merit Merit { get; set; }
    }
}
