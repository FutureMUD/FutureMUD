using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Cell
    {
        public Cell()
        {
            ActiveProjects = new HashSet<ActiveProject>();
            CellOverlays = new HashSet<CellOverlay>();
            CellsForagableYields = new HashSet<CellsForagableYield>();
            CellsGameItems = new HashSet<CellsGameItems>();
            CellsMagicResources = new HashSet<CellMagicResource>();
            CellsRangedCovers = new HashSet<CellsRangedCovers>();
            CellsTags = new HashSet<CellsTags>();
            CharacterLog = new HashSet<CharacterLog>();
            Characters = new HashSet<Character>();
            ClansAdministrationCells = new HashSet<ClanAdministrationCell>();
            ClansTreasuryCells = new HashSet<ClanTreasuryCell>();
            Crimes = new HashSet<Crime>();
            HooksPerceivables = new HashSet<HooksPerceivable>();
            ShopsStockroomCell = new HashSet<Shop>();
            ShopsStoreroomCells = new HashSet<ShopsStoreroomCell>();
            ShopsWorkshopCell = new HashSet<Shop>();
            Zones = new HashSet<Zone>();
            Tracks = new HashSet<Track>();
        }

        public long Id { get; set; }
        public long RoomId { get; set; }
        public long? CurrentOverlayId { get; set; }
        public long? ForagableProfileId { get; set; }
        public bool Temporary { get; set; }
        public string EffectData { get; set; }

        public virtual CellOverlay CurrentOverlay { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<ActiveProject> ActiveProjects { get; set; }
        public virtual ICollection<CellOverlay> CellOverlays { get; set; }
        public virtual ICollection<CellsForagableYield> CellsForagableYields { get; set; }
        public virtual ICollection<CellsGameItems> CellsGameItems { get; set; }
        public virtual ICollection<CellMagicResource> CellsMagicResources { get; set; }
        public virtual ICollection<CellsRangedCovers> CellsRangedCovers { get; set; }
        public virtual ICollection<CellsTags> CellsTags { get; set; }
        public virtual ICollection<CharacterLog> CharacterLog { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<ClanAdministrationCell> ClansAdministrationCells { get; set; }
        public virtual ICollection<ClanTreasuryCell> ClansTreasuryCells { get; set; }
        public virtual ICollection<Crime> Crimes { get; set; }
        public virtual ICollection<HooksPerceivable> HooksPerceivables { get; set; }
        public virtual ICollection<Shop> ShopsStockroomCell { get; set; }
        public virtual ICollection<ShopsStoreroomCell> ShopsStoreroomCells { get; set; }
        public virtual ICollection<Shop> ShopsWorkshopCell { get; set; }
        public virtual ICollection<Zone> Zones { get; set; }
        public virtual ICollection<Track> Tracks { get; set; }
    }
}
