using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacteristicDefinition
    {
        public CharacteristicDefinition()
        {
            CharacteristicProfiles = new HashSet<CharacteristicProfile>();
            CharacteristicValues = new HashSet<CharacteristicValue>();
            EthnicitiesCharacteristics = new HashSet<EthnicitiesCharacteristics>();
            InverseParent = new HashSet<CharacteristicDefinition>();
            RacesAdditionalCharacteristics = new HashSet<RacesAdditionalCharacteristics>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Pattern { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public int? ChargenDisplayType { get; set; }
        public string Model { get; set; }
        public string Definition { get; set; }

        public virtual CharacteristicDefinition Parent { get; set; }
        public virtual ICollection<CharacteristicProfile> CharacteristicProfiles { get; set; }
        public virtual ICollection<CharacteristicValue> CharacteristicValues { get; set; }
        public virtual ICollection<EthnicitiesCharacteristics> EthnicitiesCharacteristics { get; set; }
        public virtual ICollection<CharacteristicDefinition> InverseParent { get; set; }
        public virtual ICollection<RacesAdditionalCharacteristics> RacesAdditionalCharacteristics { get; set; }
    }
}
