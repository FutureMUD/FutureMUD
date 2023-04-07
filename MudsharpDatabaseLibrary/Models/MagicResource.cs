using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MagicResource
    {
        public MagicResource()
        {
            CellsMagicResources = new HashSet<CellMagicResource>();
            CharactersMagicResources = new HashSet<CharactersMagicResources>();
            GameItemsMagicResources = new HashSet<GameItemMagicResource>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public int MagicResourceType { get; set; }

        public virtual ICollection<CellMagicResource> CellsMagicResources { get; set; }
        public virtual ICollection<CharactersMagicResources> CharactersMagicResources { get; set; }
        public virtual ICollection<GameItemMagicResource> GameItemsMagicResources { get; set; }
    }
}
