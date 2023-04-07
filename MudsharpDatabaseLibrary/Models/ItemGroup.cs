using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ItemGroup
    {
        public ItemGroup()
        {
            GameItemProtos = new HashSet<GameItemProto>();
            ItemGroupForms = new HashSet<ItemGroupForm>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }

        public virtual ICollection<GameItemProto> GameItemProtos { get; set; }
        public virtual ICollection<ItemGroupForm> ItemGroupForms { get; set; }
    }
}
