using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesAttributes
    {
        public long RaceId { get; set; }
        public long AttributeId { get; set; }
        public bool IsHealthAttribute { get; set; }
        public double AttributeBonus { get; set; }
        public string DiceExpression { get; set; }

        public virtual TraitDefinition Attribute { get; set; }
        public virtual Race Race { get; set; }
    }
}
