using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacteristicProfile
    {
        public CharacteristicProfile()
        {
            EthnicitiesCharacteristics = new HashSet<EthnicitiesCharacteristics>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public string Type { get; set; }
        public long TargetDefinitionId { get; set; }
        public string Description { get; set; }

        public virtual CharacteristicDefinition TargetDefinition { get; set; }
        public virtual ICollection<EthnicitiesCharacteristics> EthnicitiesCharacteristics { get; set; }
    }
}
