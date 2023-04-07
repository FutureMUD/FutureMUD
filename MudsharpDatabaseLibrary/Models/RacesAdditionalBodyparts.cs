using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesAdditionalBodyparts
    {
        public string Usage { get; set; }
        public long BodypartId { get; set; }
        public long RaceId { get; set; }

        public virtual BodypartProto Bodypart { get; set; }
        public virtual Race Race { get; set; }
    }
}
