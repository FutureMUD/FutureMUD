using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class MoveSpeed
    {
        public long Id { get; set; }
        public long BodyProtoId { get; set; }
        public double Multiplier { get; set; }
        public string Alias { get; set; }
        public string FirstPersonVerb { get; set; }
        public string ThirdPersonVerb { get; set; }
        public string PresentParticiple { get; set; }
        public long PositionId { get; set; }
        public double StaminaMultiplier { get; set; }

        public virtual BodyProto BodyProto { get; set; }
    }
}
