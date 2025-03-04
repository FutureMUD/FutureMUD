using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class GameItemProto
    {
        public GameItemProto()
        {
            Appointments = new HashSet<Appointment>();
            GameItemProtosDefaultVariables = new HashSet<GameItemProtosDefaultVariable>();
            GameItemProtosGameItemComponentProtos = new HashSet<GameItemProtosGameItemComponentProtos>();
            GameItemProtosOnLoadProgs = new HashSet<GameItemProtosOnLoadProgs>();
            GameItemProtosTags = new HashSet<GameItemProtosTags>();
            Ranks = new HashSet<Rank>();
            ExtraDescriptions = new HashSet<GameItemProtoExtraDescription>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
        public long MaterialId { get; set; }
        public long EditableItemId { get; set; }
        public int RevisionNumber { get; set; }
        public int Size { get; set; }
        public double Weight { get; set; }
        public bool ReadOnly { get; set; }
        public string LongDescription { get; set; }
        public long? ItemGroupId { get; set; }
        public long? OnDestroyedGameItemProtoId { get; set; }
        public long? HealthStrategyId { get; set; }
        public int BaseItemQuality { get; set; }
        public string CustomColour { get; set; }
        public bool HighPriority { get; set; }
        public long? MorphGameItemProtoId { get; set; }
        public int MorphTimeSeconds { get; set; }
        public string MorphEmote { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public bool PermitPlayerSkins { get; set; }
        public decimal CostInBaseCurrency { get; set; }
        public bool IsHiddenFromPlayers { get; set; }
        public bool PreserveRegisterVariables { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ItemGroup ItemGroup { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<GameItemProtosDefaultVariable> GameItemProtosDefaultVariables { get; set; }
        public virtual ICollection<GameItemProtosGameItemComponentProtos> GameItemProtosGameItemComponentProtos { get; set; }
        public virtual ICollection<GameItemProtosOnLoadProgs> GameItemProtosOnLoadProgs { get; set; }
        public virtual ICollection<GameItemProtosTags> GameItemProtosTags { get; set; }
        public virtual ICollection<Rank> Ranks { get; set; }
        public virtual ICollection<GameItemProtoExtraDescription> ExtraDescriptions { get; set; }
    }
}
