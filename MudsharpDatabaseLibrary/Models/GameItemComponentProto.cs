using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemComponentProto
    {
        public GameItemComponentProto()
        {
            GameItemProtosGameItemComponentProtos = new HashSet<GameItemProtosGameItemComponentProtos>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public long EditableItemId { get; set; }
        public string Description { get; set; }
        public int RevisionNumber { get; set; }
        public string Name { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ICollection<GameItemProtosGameItemComponentProtos> GameItemProtosGameItemComponentProtos { get; set; }
    }
}
