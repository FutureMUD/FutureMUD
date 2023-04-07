using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItem
    {
        public GameItem()
        {
            BodiesGameItems = new HashSet<BodiesGameItems>();
            BodiesImplants = new HashSet<BodiesImplants>();
            BodiesProsthetics = new HashSet<BodiesProsthetics>();
            CellsGameItems = new HashSet<CellsGameItems>();
            GameItemComponents = new HashSet<GameItemComponent>();
            GameItemsMagicResources = new HashSet<GameItemMagicResource>();
            HooksPerceivables = new HashSet<HooksPerceivable>();
            InverseContainer = new HashSet<GameItem>();
            Merchandises = new HashSet<Merchandise>();
            ShopsTills = new HashSet<ShopsTill>();
            WoundsGameItem = new HashSet<Wound>();
            WoundsLodgedItem = new HashSet<Wound>();
            WoundsToolOrigin = new HashSet<Wound>();
        }

        public long Id { get; set; }
        public int Quality { get; set; }
        public long GameItemProtoId { get; set; }
        public int GameItemProtoRevision { get; set; }
        public int RoomLayer { get; set; }
        public double Condition { get; set; }
        public long MaterialId { get; set; }
        public int Size { get; set; }
        public long? ContainerId { get; set; }
        public int PositionId { get; set; }
        public int PositionModifier { get; set; }
        public long? PositionTargetId { get; set; }
        public string PositionTargetType { get; set; }
        public string PositionEmote { get; set; }
        public int? MorphTimeRemaining { get; set; }
        public string EffectData { get; set; }
        public long? SkinId { get; set; }

        public virtual GameItem Container { get; set; }
        public virtual ICollection<BodiesGameItems> BodiesGameItems { get; set; }
        public virtual ICollection<BodiesImplants> BodiesImplants { get; set; }
        public virtual ICollection<BodiesProsthetics> BodiesProsthetics { get; set; }
        public virtual ICollection<CellsGameItems> CellsGameItems { get; set; }
        public virtual ICollection<GameItemComponent> GameItemComponents { get; set; }
        public virtual ICollection<GameItemMagicResource> GameItemsMagicResources { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<GameItem> InverseContainer { get; set; }
        public virtual ICollection<Merchandise> Merchandises { get; set; }
        public virtual ICollection<ShopsTill> ShopsTills { get; set; }
        public virtual ICollection<Wound> WoundsGameItem { get; set; }
        public virtual ICollection<Wound> WoundsLodgedItem { get; set; }
        public virtual ICollection<Wound> WoundsToolOrigin { get; set; }
    }
}
