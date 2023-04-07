using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class EthnicitiesCharacteristics
    {
        public long EthnicityId { get; set; }
        public long CharacteristicDefinitionId { get; set; }
        public long CharacteristicProfileId { get; set; }

        public virtual CharacteristicDefinition CharacteristicDefinition { get; set; }
        public virtual CharacteristicProfile CharacteristicProfile { get; set; }
        public virtual Ethnicity Ethnicity { get; set; }
    }
}
