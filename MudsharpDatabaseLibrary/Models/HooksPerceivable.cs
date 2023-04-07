using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class HooksPerceivable
    {
        public long Id { get; set; }
        public long HookId { get; set; }
        public long? BodyId { get; set; }
        public long? CharacterId { get; set; }
        public long? GameItemId { get; set; }
        public long? CellId { get; set; }
        public long? ZoneId { get; set; }
        public long? ShardId { get; set; }

        public virtual Body Body { get; set; }
        public virtual Cell Cell { get; set; }
        public virtual Character Character { get; set; }
        public virtual GameItem GameItem { get; set; }
        public virtual Hooks Hook { get; set; }
        public virtual Shard Shard { get; set; }
        public virtual Zone Zone { get; set; }
    }
}
