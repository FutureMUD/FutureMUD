using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Infection
    {
        public long Id { get; set; }
        public int InfectionType { get; set; }
        public int Virulence { get; set; }
        public double Intensity { get; set; }
        public long OwnerId { get; set; }
        public long? WoundId { get; set; }
        public long? BodypartId { get; set; }
        public double Immunity { get; set; }

        public virtual BodypartProto Bodypart { get; set; }
        public virtual Body Owner { get; set; }
        public virtual Wound Wound { get; set; }
    }
}
