using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Limb
    {
        public Limb()
        {
            LimbsBodypartProto = new HashSet<LimbBodypartProto>();
            LimbsSpinalParts = new HashSet<LimbsSpinalPart>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long RootBodypartId { get; set; }
        public int LimbType { get; set; }
        public long RootBodyId { get; set; }
        public double LimbDamageThresholdMultiplier { get; set; }
        public double LimbPainThresholdMultiplier { get; set; }

        public virtual BodyProto RootBody { get; set; }
        public virtual BodypartProto RootBodypart { get; set; }
        public virtual ICollection<LimbBodypartProto> LimbsBodypartProto { get; set; }
        public virtual ICollection<LimbsSpinalPart> LimbsSpinalParts { get; set; }
    }
}
