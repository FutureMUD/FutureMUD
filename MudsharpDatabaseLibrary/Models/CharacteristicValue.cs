using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacteristicValue
    {
        public CharacteristicValue()
        {
            Characteristics = new HashSet<Characteristic>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long DefinitionId { get; set; }
        public string Value { get; set; }
        public bool Default { get; set; }
        public string AdditionalValue { get; set; }
        public long? FutureProgId { get; set; }
        public long? OngoingValidityProgId { get; set; }
        public int Pluralisation { get; set; }

        public virtual CharacteristicDefinition Definition { get; set; }
        public virtual FutureProg FutureProg { get; set; }
        public virtual FutureProg OngoingValidityProg { get; set; }
        public virtual ICollection<Characteristic> Characteristics { get; set; }
    }
}
