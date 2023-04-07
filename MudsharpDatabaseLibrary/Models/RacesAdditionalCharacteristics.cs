using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RacesAdditionalCharacteristics
    {
        public long RaceId { get; set; }
        public long CharacteristicDefinitionId { get; set; }
        public string Usage { get; set; }

        public virtual CharacteristicDefinition CharacteristicDefinition { get; set; }
        public virtual Race Race { get; set; }
    }
}
