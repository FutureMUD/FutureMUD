using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BoneOrganCoverage
    {
        public long BoneId { get; set; }
        public long OrganId { get; set; }
        public double CoverageChance { get; set; }

        public virtual BodypartProto Bone { get; set; }
        public virtual BodypartProto Organ { get; set; }
    }
}
