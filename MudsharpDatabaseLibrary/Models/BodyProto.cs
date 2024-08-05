using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodyProto
    {
        public BodyProto()
        {
            BodyProtosAdditionalBodyparts = new HashSet<BodyProtosAdditionalBodyparts>();
            BodypartGroupDescribersBodyProtos = new HashSet<BodypartGroupDescribersBodyProtos>();
            BodypartProtos = new HashSet<BodypartProto>();
            ButcheryProducts = new HashSet<ButcheryProducts>();
            InverseCountsAs = new HashSet<BodyProto>();
            Limbs = new HashSet<Limb>();
            MoveSpeeds = new HashSet<MoveSpeed>();
            Races = new HashSet<Race>();
            BodyProtosPositions = new HashSet<BodyProtosPositions>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? CountsAsId { get; set; }
        public long WearSizeParameterId { get; set; }
        public string WielderDescriptionPlural { get; set; }
        public string WielderDescriptionSingle { get; set; }
        public string ConsiderString { get; set; }
        public long? StaminaRecoveryProgId { get; set; }
        public int MinimumLegsToStand { get; set; }
        public int MinimumWingsToFly { get; set; }
        public string LegDescriptionSingular { get; set; }
        public string LegDescriptionPlural { get; set; }
        public long? DefaultSmashingBodypartId { get; set; }
        public string NameForTracking { get; set; }

        public virtual BodyProto CountsAs { get; set; }
        public virtual BodypartProto DefaultSmashingBodypart { get; set; }
        public virtual WearableSizeParameterRule WearSizeParameter { get; set; }
        public virtual ICollection<BodyProtosAdditionalBodyparts> BodyProtosAdditionalBodyparts { get; set; }
        public virtual ICollection<BodypartGroupDescribersBodyProtos> BodypartGroupDescribersBodyProtos { get; set; }
        public virtual ICollection<BodypartProto> BodypartProtos { get; set; }
        public virtual ICollection<ButcheryProducts> ButcheryProducts { get; set; }
        public virtual ICollection<BodyProto> InverseCountsAs { get; set; }
        public virtual ICollection<Limb> Limbs { get; set; }
        public virtual ICollection<MoveSpeed> MoveSpeeds { get; set; }
        public virtual ICollection<Race> Races { get; set; }
        public virtual ICollection<BodyProtosPositions> BodyProtosPositions { get; set; }
    }
}
