using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WearableSizeParameterRule
    {
        public WearableSizeParameterRule()
        {
            BodyProtos = new HashSet<BodyProto>();
        }

        public long Id { get; set; }
        public double MinHeightFactor { get; set; }
        public double MaxHeightFactor { get; set; }
        public double MinWeightFactor { get; set; }
        public double? MaxWeightFactor { get; set; }
        public double? MinTraitFactor { get; set; }
        public double? MaxTraitFactor { get; set; }
        public long? TraitId { get; set; }
        public long BodyProtoId { get; set; }
        public bool IgnoreTrait { get; set; }
        public string WeightVolumeRatios { get; set; }
        public string TraitVolumeRatios { get; set; }
        public string HeightLinearRatios { get; set; }

        public virtual TraitDefinition Trait { get; set; }
        public virtual ICollection<BodyProto> BodyProtos { get; set; }
    }
}
