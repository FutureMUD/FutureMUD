using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ArmourType
    {
        public ArmourType()
        {
            BodypartProto = new HashSet<BodypartProto>();
            Races = new HashSet<Race>();
            ShieldTypes = new HashSet<ShieldType>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int MinimumPenetrationDegree { get; set; }
        public int BaseDifficultyDegrees { get; set; }
        public int StackedDifficultyDegrees { get; set; }
        public string Definition { get; set; }

        public virtual ICollection<BodypartProto> BodypartProto { get; set; }
        public virtual ICollection<Race> Races { get; set; }
        public virtual ICollection<ShieldType> ShieldTypes { get; set; }
    }
}
