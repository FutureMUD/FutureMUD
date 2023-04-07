using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Tag
    {
        public Tag()
        {
            CellsTags = new HashSet<CellsTags>();
            GameItemProtosTags = new HashSet<GameItemProtosTags>();
            GasesTags = new HashSet<GasesTags>();
            InverseParent = new HashSet<Tag>();
            LiquidsTags = new HashSet<LiquidsTags>();
            MaterialsTags = new HashSet<MaterialsTags>();
            RaceButcheryProfiles = new HashSet<RaceButcheryProfile>();
        }

        public string Name { get; set; }
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public long? ShouldSeeProgId { get; set; }

        public virtual Tag Parent { get; set; }
        public virtual FutureProg ShouldSeeProg { get; set; }
        public virtual ICollection<CellsTags> CellsTags { get; set; }
        public virtual ICollection<GameItemProtosTags> GameItemProtosTags { get; set; }
        public virtual ICollection<GasesTags> GasesTags { get; set; }
        public virtual ICollection<Tag> InverseParent { get; set; }
        public virtual ICollection<LiquidsTags> LiquidsTags { get; set; }
        public virtual ICollection<MaterialsTags> MaterialsTags { get; set; }
        public virtual ICollection<RaceButcheryProfile> RaceButcheryProfiles { get; set; }
    }
}
