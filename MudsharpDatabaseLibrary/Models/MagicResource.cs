using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicResource
    {
        public MagicResource()
        {
            RoomsMagicResources = new HashSet<RoomMagicResource>();
            CharactersMagicResources = new HashSet<CharactersMagicResources>();
            GameItemsMagicResources = new HashSet<GameItemMagicResource>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public int MagicResourceType { get; set; }
        public string BottomColour { get; set; }
        public string MidColour { get; set; }
        public string TopColour { get; set; }

        public virtual ICollection<RoomMagicResource> RoomsMagicResources { get; set; }
        public virtual ICollection<CharactersMagicResources> CharactersMagicResources { get; set; }
        public virtual ICollection<GameItemMagicResource> GameItemsMagicResources { get; set; }
    }
}
